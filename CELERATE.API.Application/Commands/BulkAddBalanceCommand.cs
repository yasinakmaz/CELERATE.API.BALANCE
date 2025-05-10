using MediatR;

namespace CELERATE.API.Application.Commands
{
    public class BulkAddBalanceCommand : IRequest<List<BulkAddBalanceResult>>
    {
        public List<BulkBalanceItem>? Items { get; set; }
        public string? OperatorId { get; set; }
        public string? BranchId { get; set; }
    }
}
