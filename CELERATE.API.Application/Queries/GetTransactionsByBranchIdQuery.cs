using CELERATE.API.Application.DTOs;
using MediatR;

namespace CELERATE.API.Application.Queries
{
    public class GetTransactionsByBranchIdQuery : IRequest<List<TransactionDto>>
    {
        public string? BranchId { get; set; }
    }
}
