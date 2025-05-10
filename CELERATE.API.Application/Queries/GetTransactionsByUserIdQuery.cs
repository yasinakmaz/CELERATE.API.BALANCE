using CELERATE.API.Application.DTOs;
using MediatR;

namespace CELERATE.API.Application.Queries
{
    public class GetTransactionsByUserIdQuery : IRequest<List<TransactionDto>>
    {
        public string? UserId { get; set; }
    }
}
