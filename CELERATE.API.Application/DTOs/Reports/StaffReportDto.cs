namespace CELERATE.API.Application.DTOs.Reports
{
    public class StaffReportDto
    {
        public string? StaffId { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? BranchId { get; set; }
        public string? BranchName { get; set; }
        public decimal AddBalanceTotal { get; set; }
        public decimal SpendBalanceTotal { get; set; }
        public decimal Total { get; set; }
    }
}
