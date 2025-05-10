namespace CELERATE.API.Infrastructure.Firebase.Logging
{
    public class LogEntry
    {
        public string? Id { get; set; }
        public string? UserId { get; set; }
        public string? Action { get; set; }
        public string? Details { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? BranchId { get; set; }
    }
}
