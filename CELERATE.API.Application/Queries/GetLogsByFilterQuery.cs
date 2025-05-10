using CELERATE.API.Application.DTOs;
using MediatR;

namespace CELERATE.API.Application.Queries
{
    public class GetLogsByFilterQuery : IRequest<List<LogEntryDto>>
    {
        public string? UserId { get; set; }
        public string? BranchId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Action { get; set; }
    }
}
