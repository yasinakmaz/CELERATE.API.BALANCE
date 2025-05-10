namespace CELERATE.API.Application.Commands
{
    public class BulkAddBalanceResult
    {
        public string? UserId { get; set; }
        public string? NfcCardId { get; set; }
        public decimal Amount { get; set; }
        public decimal NewBalance { get; set; }
        public string? BranchId { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
