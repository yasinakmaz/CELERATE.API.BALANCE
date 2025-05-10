using CELERATE.API.Application.DTOs.ReactOptimized;
using MediatR;

namespace CELERATE.API.Application.Queries
{
    public class GetCustomersListQuery : IRequest<CustomerListDto>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SearchTerm { get; set; }
    }
}
