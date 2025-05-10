using CELERATE.API.Application.DTOs;
using MediatR;

namespace CELERATE.API.Application.Queries
{
    public class GetUserByIdQuery : IRequest<UserDto>
    {
        public string? Id { get; set; }
    }
}
