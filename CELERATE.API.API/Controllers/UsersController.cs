using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CELERATE.API.Application.Commands;
using CELERATE.API.Application.Queries;
using CELERATE.API.Application.DTOs;

namespace CELERATE.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("customers")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _mediator.Send(new GetAllCustomersQuery());
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var user = await _mediator.Send(new GetUserByIdQuery { Id = id });

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }
    }
}
