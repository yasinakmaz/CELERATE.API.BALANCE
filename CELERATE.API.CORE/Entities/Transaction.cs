using Google.Cloud.Firestore;

namespace CELERATE.API.CORE.Entities
{
    [FirestoreData]
    public class Transaction
    {
        // Boş constructor ekleme (Firestore için gerekli)
        public Transaction() { }

        // Mevcut constructor'ı koruyun
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

        [FirestoreProperty]
        public string Id { get; set; }

        [FirestoreProperty]
        public string CardId { get; set; }

        [FirestoreProperty]
        public string UserId { get; set; }

        [FirestoreProperty]
        public string OperatorId { get; set; }

        [FirestoreProperty]
        public string BranchId { get; set; }

        [FirestoreProperty]
        public TransactionType Type { get; set; }

        [FirestoreProperty]
        public decimal Amount { get; set; }

        [FirestoreProperty]
        public decimal BalanceAfter { get; set; }

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }
    }

    public enum TransactionType
    {
        AddBalance,
        SpendBalance
    }
}