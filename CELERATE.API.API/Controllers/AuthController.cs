using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CELERATE.API.Application.Commands;
using CELERATE.API.Application.Queries;
using CELERATE.API.Application.DTOs;

namespace CELERATE.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticateByCardCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new
            {
                token = result.Token,
                userId = result.UserId,
                fullName = result.UserFullName,
                role = result.UserRole,
                permissions = result.Permissions
            });
        }
    }
}
