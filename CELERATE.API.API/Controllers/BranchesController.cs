using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CELERATE.API.Application.Commands;
using CELERATE.API.Application.Queries;
using CELERATE.API.Application.DTOs;

namespace CELERATE.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BranchesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BranchesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Policy = "CreateBranch")]
        public async Task<IActionResult> CreateBranch([FromBody] CreateBranchCommand command)
        {
            var branchId = await _mediator.Send(command);
            return Ok(new { id = branchId });
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBranches()
        {
            var branches = await _mediator.Send(new GetAllBranchesQuery());
            return Ok(branches);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBranchById(string id)
        {
            var branch = await _mediator.Send(new GetBranchByIdQuery { Id = id });

            if (branch == null)
            {
                return NotFound();
            }

            return Ok(branch);
        }
    }
}
