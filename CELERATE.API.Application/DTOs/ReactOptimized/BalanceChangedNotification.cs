namespace CELERATE.API.Application.DTOs.ReactOptimized
{
    public class BalanceChangedNotification
    {
        public string? UserId { get; set; }
        public decimal OldBalance { get; set; }
        public decimal NewBalance { get; set; }
        public string? TransactionType { get; set; }
    }
}
