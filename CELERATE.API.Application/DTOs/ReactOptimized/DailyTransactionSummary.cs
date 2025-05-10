namespace CELERATE.API.Application.DTOs.ReactOptimized
{
    public class DailyTransactionSummary
    {
        public DateTime Date { get; set; }
        public decimal TotalAdded { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
