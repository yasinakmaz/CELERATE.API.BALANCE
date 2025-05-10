// CELERATE.API.Application/Handlers/AddBalanceHandler.cs
using CELERATE.API.Application.Commands;
using CELERATE.API.CORE.Entities;
using CELERATE.API.CORE.Interfaces;
using CELERATE.API.Application.Models;
using MediatR;

namespace CELERATE.API.Application.Handlers
{
    public class AddBalanceHandler : IRequestHandler<AddBalanceCommand, decimal>
    {
        private readonly ICardRepository _cardRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly ILogRepository _logRepository;

        public AddBalanceHandler(
            ICardRepository cardRepository,
            ITransactionRepository transactionRepository,
            IBranchRepository branchRepository,
            ILogRepository logRepository)
        {
            _cardRepository = cardRepository;
            _transactionRepository = transactionRepository;
            _branchRepository = branchRepository;
            _logRepository = logRepository;
        }

        public async Task<decimal> Handle(AddBalanceCommand request, CancellationToken cancellationToken)
        {
            var startTime = DateTime.UtcNow;

            // Kartı bul
            var card = await _cardRepository.GetByNfcIdAsync(request.NfcCardId);
            if (card == null)
            {
                throw new ApplicationException("Kart bulunamadı.");
            }

            // Şube kontrolü
            var branch = await _branchRepository.GetByIdAsync(request.BranchId);
            if (branch == null)
            {
                throw new ApplicationException("Şube bulunamadı.");
            }

            // Şubenin bakiye yükleme yetkisi var mı?
            if (branch.OperationType != BranchOperationType.AddBalance &&
                branch.OperationType != BranchOperationType.Both)
            {
                throw new ApplicationException("Bu şubeden bakiye yükleme işlemi yapılamaz.");
            }

            // Bakiye ekle
            card.AddBalance(request.Amount);
            await _cardRepository.UpdateAsync(card);

            // İşlem kaydı oluştur
            var transaction = new Transaction(
                Guid.NewGuid().ToString(),
                card.Id,
                card.UserId,
                request.OperatorId,
                request.BranchId,
                TransactionType.AddBalance,
                request.Amount,
                card.Balance
            );

            await _transactionRepository.AddAsync(transaction);

            // Log kaydı
            var endTime = DateTime.UtcNow;
            await _logRepository.LogActionAsync(
                request.OperatorId,
                "AddBalance",
                $"Bakiye yükleme: {request.Amount:C} - Kart: {request.NfcCardId}",
                startTime,
                endTime,
                request.BranchId
            );

            return card.Balance;
        }
    }
}