using CELERATE.API.Application.DTOs;
using MediatR;

namespace CELERATE.API.Application.Queries
{
    public class GetAllStaffQuery : IRequest<List<UserDto>>
    {
    }
}
