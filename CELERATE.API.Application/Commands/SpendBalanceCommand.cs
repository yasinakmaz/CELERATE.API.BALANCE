using MediatR;

namespace CELERATE.API.Application.Commands
{
    public class SpendBalanceCommand : IRequest<decimal>
    {
        public string? NfcCardId { get; set; }
        public decimal Amount { get; set; }
        public string? OperatorId { get; set; }
        public string? BranchId { get; set; }
    }
}
