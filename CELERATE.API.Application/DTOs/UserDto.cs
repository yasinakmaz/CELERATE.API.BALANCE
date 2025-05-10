namespace CELERATE.API.Application.DTOs
{
    public class UserDto
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? TcIdentityNumber { get; set; }
        public string? Gender { get; set; }
        public int Age { get; set; }
        public string? UserType { get; set; }
        public string? UserRole { get; set; }
        public bool IsActive { get; set; }
        public string? BranchId { get; set; }
        public string? BranchName { get; set; }
        public string? CardId { get; set; }
        public decimal Balance { get; set; }
    }

}
