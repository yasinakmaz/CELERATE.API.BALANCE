namespace CELERATE.API.Application.DTOs
{
    public class LogEntryDto
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? Action { get; set; }
        public string? Details { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? BranchId { get; set; }
        public string? BranchName { get; set; }
    }
}
