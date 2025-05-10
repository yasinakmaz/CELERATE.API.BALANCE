using CELERATE.API.Application.DTOs;
using MediatR;

namespace CELERATE.API.Application.Queries
{
    public class GetCustomerDetailsQuery : IRequest<CustomerDetailsDto>
    {
        public string? UserId { get; set; }
    }

    public class CustomerDetailsDto
    {
        public UserDto? User { get; set; }
        public CardDto? Card { get; set; }
        public List<TransactionDto>? Transactions { get; set; }
    }
}
