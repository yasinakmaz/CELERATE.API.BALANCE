using MediatR;

namespace CELERATE.API.Application.Queries
{
    public class GetBranchesForDropdownQuery : IRequest<List<DropdownItemDto>>
    {
    }

    public class DropdownItemDto
    {
        public string? Value { get; set; }
        public string? Label { get; set; }
    }
}
