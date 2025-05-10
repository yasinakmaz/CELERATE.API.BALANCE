namespace CELERATE.API.Application.DTOs
{
    public class TransactionDto
    {
        public string? Id { get; set; }
        public string? CardId { get; set; }
        public string? UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? OperatorId { get; set; }
        public string? OperatorFullName { get; set; }
        public string? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? Type { get; set; }
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
