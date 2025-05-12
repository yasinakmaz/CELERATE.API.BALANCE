using CELERATE.API.Application.Commands;
namespace CELERATE.API.Application.Handlers
{
    public class CreateAuthorizedCardHandler : IRequestHandler<CreateAuthorizedCardCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly ILogRepository _logRepository;

        public CreateAuthorizedCardHandler(
            IUserRepository userRepository,
            ICardRepository cardRepository,
            IBranchRepository branchRepository,
            ILogRepository logRepository)
        {
            _userRepository = userRepository;
            _cardRepository = cardRepository;
            _branchRepository = branchRepository;
            _logRepository = logRepository;
        }

        public async Task<string> Handle(CreateAuthorizedCardCommand request, CancellationToken cancellationToken)
        {
            var startTime = DateTime.UtcNow;

            // TC kimlik kontrolü
            var existingUserByTC = await _userRepository.GetByTcIdentityNumberAsync(request.TcIdentityNumber);
            if (existingUserByTC != null)
            {
                throw new ApplicationException("Bu TC kimlik numarası ile kayıtlı bir kullanıcı bulunmaktadır.");
            }

            // NFC kart kontrolü
            var existingCard = await _cardRepository.GetByNfcIdAsync(request.NfcCardId);
            if (existingCard != null)
            {
                throw new ApplicationException("Bu NFC kart ID'si zaten kullanımda.");
            }

            // Şube kontrolü
            var branch = await _branchRepository.GetByIdAsync(request.BranchId);
            if (branch == null)
            {
                throw new ApplicationException("Belirtilen şube bulunamadı.");
            }

            // User oluştur
            var userId = Guid.NewGuid().ToString();
            var user = new User(
                userId,
                request.FullName,
                request.PhoneNumber,
                request.TcIdentityNumber,
                Enum.Parse<Gender>(request.Gender),
                request.Age,
                UserType.Staff, // Yetkili kart için Staff olarak ayarla
                "", // Önce kartı oluşturup sonra bağlayacağız
                request.BranchId
            );

            await _userRepository.AddAsync(user);

            // Kart oluştur
            var cardId = Guid.NewGuid().ToString();
            var card = new Card(
                cardId,
                request.NfcCardId,
                userId,
                true // Yetkili kart
            );

            // İzinleri ekle
            if (request.Permissions != null)
            {
                foreach (var permission in request.Permissions)
                {
                    if (Enum.TryParse<Permission>(permission, out var permEnum))
                    {
                        card.AddPermission(permEnum);
                    }
                }
            }

            await _cardRepository.AddAsync(card);

            user = new User(
                userId,
                request.FullName,
                request.PhoneNumber,
                request.TcIdentityNumber,
                Enum.Parse<Gender>(request.Gender),
                request.Age,
                UserType.Staff,
                cardId,
                request.BranchId
            );

            await _userRepository.UpdateAsync(user);

            var endTime = DateTime.UtcNow;
            await _logRepository.LogActionAsync(
                userId,
                "CreateAuthorizedCard",
                $"Yeni yetkili kart oluşturuldu: {request.FullName}",
                startTime,
                endTime,
                request.BranchId
            );

            return cardId;
        }
    }
}