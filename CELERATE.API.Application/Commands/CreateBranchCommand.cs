using MediatR;

namespace CELERATE.API.Application.Commands
{
    public class CreateBranchCommand : IRequest<string>
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }
        public string? CompanyType { get; set; }
        public string? TaxOffice { get; set; }
        public string? TaxNumber { get; set; }
        public string? IdentityNumber { get; set; }
        public string? Address { get; set; }
        public string? OperationType { get; set; }
    }
}
