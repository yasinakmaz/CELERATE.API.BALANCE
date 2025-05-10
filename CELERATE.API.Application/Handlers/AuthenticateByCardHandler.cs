using CELERATE.API.Application.Commands;
using CELERATE.API.Application.Models;
using CELERATE.API.CORE.Entities;
using CELERATE.API.CORE.Interfaces;
using MediatR;

namespace CELERATE.API.Application.Handlers
{
    public class AuthenticateByCardHandler : IRequestHandler<AuthenticateByCardCommand, AuthenticationResult>
    {
        private readonly ICardRepository _cardRepository;
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly ILogRepository _logRepository;

        public AuthenticateByCardHandler(
            ICardRepository cardRepository,
            IUserRepository userRepository,
            IJwtTokenGenerator tokenGenerator,
            ILogRepository logRepository)
        {
            _cardRepository = cardRepository;
            _userRepository = userRepository;
            _tokenGenerator = tokenGenerator;
            _logRepository = logRepository;
        }

        public async Task<AuthenticationResult> Handle(AuthenticateByCardCommand request, CancellationToken cancellationToken)
        {
            var startTime = DateTime.UtcNow;

            // NFC kartını bul
            var card = await _cardRepository.GetByNfcIdAsync(request.NfcCardId);
            if (card == null)
            {
                return new AuthenticationResult { Succeeded = false, ErrorMessage = "Kart bulunamadı." };
            }

            // Yetkili kart mı kontrol et
            if (!card.IsAuthorized)
            {
                return new AuthenticationResult { Succeeded = false, ErrorMessage = "MÜŞTERİ KARTI İLE GİRİŞ YAPILAMAZ LÜTFEN YETKİLİ KARTI İLE GİRİŞ YAPINIZ" };
            }

            // Login yetkisi var mı kontrol et
            if (!card.Permissions.Contains(Permission.Login))
            {
                return new AuthenticationResult { Succeeded = false, ErrorMessage = "Bu kartın giriş yapma yetkisi bulunmamaktadır." };
            }

            var user = await _userRepository.GetByCardIdAsync(card.Id);
            if (user == null)
            {
                return new AuthenticationResult { Succeeded = false, ErrorMessage = "Kullanıcı bulunamadı." };
            }

            // JWT token oluştur
            var token = _tokenGenerator.GenerateToken(user, card.Permissions.ToList());

            // Giriş logunu kaydet
            var endTime = DateTime.UtcNow;
            await _logRepository.LogActionAsync(
                user.Id,
                "Login",
                $"NFC kart ile giriş yapıldı: {card.NfcId}",
                startTime,
                endTime,
                user.BranchId);

            return new AuthenticationResult
            {
                Succeeded = true,
                Token = token,
                UserId = user.Id,
                UserFullName = user.FullName,
                UserRole = user.UserRole.ToString(),
                Permissions = card.Permissions.Select(p => p.ToString()).ToList()
            };
        }
    }
}
