using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CELERATE.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetTransactionsByUserId(string userId)
        {
            var transactions = await _mediator.Send(new GetTransactionsByUserIdQuery { UserId = userId });
            return Ok(transactions);
        }

        [HttpGet("by-branch/{branchId}")]
        public async Task<IActionResult> GetTransactionsByBranchId(string branchId)
        {
            var transactions = await _mediator.Send(new GetTransactionsByBranchIdQuery { BranchId = branchId });
            return Ok(transactions);
        }

        [HttpGet("by-operator/{operatorId}")]
        public async Task<IActionResult> GetTransactionsByOperatorId(string operatorId)
        {
            var transactions = await _mediator.Send(new GetTransactionsByOperatorIdQuery { OperatorId = operatorId });
            return Ok(transactions);
        }

        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetTransactionsByDateRange(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            var transactions = await _mediator.Send(new GetTransactionsByDateRangeQuery
            {
                StartDate = startDate,
                EndDate = endDate
            });

            return Ok(transactions);
        }
    }
}
