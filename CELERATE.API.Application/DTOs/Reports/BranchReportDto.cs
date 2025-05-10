namespace CELERATE.API.Application.DTOs.Reports
{
    public class BranchReportDto
    {
        public string? BranchId { get; set; }
        public string? BranchName { get; set; }
        public decimal AddBalanceTotal { get; set; }
        public decimal SpendBalanceTotal { get; set; }
        public decimal Total { get; set; }
    }
}
