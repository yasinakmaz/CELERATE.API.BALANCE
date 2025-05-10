namespace CELERATE.API.Application.DTOs
{
    public class CardDto
    {
        public string? Id { get; set; }
        public string? NfcId { get; set; }
        public string? UserId { get; set; }
        public string? UserFullName { get; set; }
        public decimal Balance { get; set; }
        public bool IsAuthorized { get; set; }
        public bool IsActive { get; set; }
        public List<string>? Permissions { get; set; }
    }
}
