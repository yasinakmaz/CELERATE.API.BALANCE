using MediatR;

namespace CELERATE.API.Application.Commands
{
    public class CreateAuthorizedCardCommand : IRequest<string>
    {
        public string? FullName { get; set; }
        public string? TcIdentityNumber { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public int Age { get; set; }
        public string? NfcCardId { get; set; }
        public string? BranchId { get; set; }
        public List<string>? Permissions { get; set; }
    }
}
