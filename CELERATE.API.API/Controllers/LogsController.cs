using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CELERATE.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LogsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LogsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Authorize(Policy = "ViewLogs")]
        public async Task<IActionResult> GetLogsByFilter(
            [FromQuery] string userId = null,
            [FromQuery] string branchId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string action = null)
        {
            var logs = await _mediator.Send(new GetLogsByFilterQuery
            {
                UserId = userId,
                BranchId = branchId,
                StartDate = startDate,
                EndDate = endDate,
                Action = action
            });

            return Ok(logs);
        }
    }
}
