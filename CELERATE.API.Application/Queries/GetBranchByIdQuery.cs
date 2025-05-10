using CELERATE.API.Application.DTOs;
using MediatR;

namespace CELERATE.API.Application.Queries
{
    public class GetBranchByIdQuery : IRequest<BranchDto>
    {
        public string? Id { get; set; }
    }
}
