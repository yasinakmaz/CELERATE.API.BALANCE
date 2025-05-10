namespace CELERATE.API.Application.DTOs.ReactOptimized
{
    public class DashboardSummaryDto
    {
        public int TotalUsers { get; set; }
        public int TotalCards { get; set; }
        public decimal TotalBalanceAdded { get; set; }
        public decimal TotalBalanceSpent { get; set; }
        public List<DailyTransactionSummary> DailyTransactions { get; set; }
    }
}
