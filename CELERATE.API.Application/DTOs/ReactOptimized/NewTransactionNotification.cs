namespace CELERATE.API.Application.DTOs.ReactOptimized
{
    public class NewTransactionNotification
    {
        public string? TransactionId { get; set; }
        public string? UserId { get; set; }
        public string? OperatorId { get; set; }
        public string? BranchId { get; set; }
        public string? Type { get; set; }
        public decimal Amount { get; set; }
    }
}
