namespace CELERATE.API.Application.Models
{
    public class AuthenticationResult
    {
        public bool Succeeded { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Token { get; set; }
        public string? UserId { get; set; }
        public string? UserFullName { get; set; }
        public string? UserRole { get; set; }
        public List<string>? Permissions { get; set; }
    }
}
