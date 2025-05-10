namespace CELERATE.API.CORE.Entities
{
    public class Transaction
    {
        public string Id { get; private set; }
        public string CardId { get; private set; }
        public string UserId { get; private set; }
        public string OperatorId { get; private set; }
        public string BranchId { get; private set; }
        public TransactionType Type { get; private set; }
        public decimal Amount { get; private set; }
        public decimal BalanceAfter { get; private set; }
        public DateTime CreatedAt { get; private set; }

        // Yapıcı metod
        public Transaction(string id, string cardId, string userId, string operatorId,
            string branchId, TransactionType type, decimal amount, decimal balanceAfter)
        {
            Id = id;
            CardId = cardId;
            UserId = userId;
            OperatorId = operatorId;
            BranchId = branchId;
            Type = type;
            Amount = amount;
            BalanceAfter = balanceAfter;
            CreatedAt = DateTime.UtcNow;
        }
    }

    public enum TransactionType
    {
        AddBalance,
        SpendBalance
    }
}
