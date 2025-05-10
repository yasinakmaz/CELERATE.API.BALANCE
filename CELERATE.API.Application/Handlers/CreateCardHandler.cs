using CELERATE.API.Application.Commands;
using MediatR;

namespace CELERATE.API.Application.Handlers
{
    public class CreateCardHandler : IRequestHandler<CreateCardCommand, string>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICardRepository _cardRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly ILogRepository _logRepository;

        public CreateCardHandler(
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

        public async Task<string> Handle(CreateCardCommand request, CancellationToken cancellationToken)
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

            // Kullanıcı oluştur
            var userId = Guid.NewGuid().ToString();
            var user = new User(
                userId,
                request.FullName,
                request.PhoneNumber,
                request.TcIdentityNumber,
                Enum.Parse<Gender>(request.Gender),
                request.Age,
                Enum.Parse<UserType>(request.UserType),
                "", // Önce kartı oluşturacağız
                request.BranchId
            );

            await _userRepository.AddAsync(user);

            // Kart oluştur
            var cardId = Guid.NewGuid().ToString();
            var card = new Card(
                cardId,
                request.NfcCardId,
                userId,
                false // Müşteri kartı, yetkili değil
            );

            await _cardRepository.AddAsync(card);

            // Kullanıcıya kartı bağla
            user = new User(
                userId,
                request.FullName,
                request.PhoneNumber,
                request.TcIdentityNumber,
                Enum.Parse<Gender>(request.Gender),
                request.Age,
                Enum.Parse<UserType>(request.UserType),
                cardId,
                request.BranchId
            );

            await _userRepository.UpdateAsync(user);

            // Log kaydı
            var endTime = DateTime.UtcNow;
            await _logRepository.LogActionAsync(
                userId,
                "CreateCard",
                $"Yeni müşteri kartı oluşturuldu: {request.FullName}",
                startTime,
                endTime,
                request.BranchId
            );

            return cardId;
        }
    }
}
