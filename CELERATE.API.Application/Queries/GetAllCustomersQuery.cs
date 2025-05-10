using CELERATE.API.Application.DTOs;
using MediatR;

namespace CELERATE.API.Application.Queries
{
    public class GetAllCustomersQuery : IRequest<List<UserDto>>
    {
    }
}
