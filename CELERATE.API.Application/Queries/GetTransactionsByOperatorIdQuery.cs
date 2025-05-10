using CELERATE.API.Application.DTOs;
using MediatR;

namespace CELERATE.API.Application.Queries
{
    public class GetTransactionsByOperatorIdQuery : IRequest<List<TransactionDto>>
    {
        public string? OperatorId { get; set; }
    }
}
