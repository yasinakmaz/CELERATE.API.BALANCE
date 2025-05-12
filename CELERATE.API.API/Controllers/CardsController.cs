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
    public class CardsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CardsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Policy = "CreateCard")]
        public async Task<IActionResult> CreateCard([FromBody] CreateCardCommand command)
        {
            var cardId = await _mediator.Send(command);
            return Ok(new { id = cardId });
        }

        [HttpPost("authorized")]
        [Authorize(Policy = "CreateAuthorizedCard")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAuthorizedCard([FromBody] CreateAuthorizedCardCommand command)
        {
            var cardId = await _mediator.Send(command);
            return Ok(new { id = cardId });
        }

        [HttpGet("{nfcId}")]
        public async Task<IActionResult> GetCardByNfcId(string nfcId)
        {
            var card = await _mediator.Send(new GetCardByNfcIdQuery { NfcId = nfcId });

            if (card == null)
            {
                return NotFound();
            }

            return Ok(card);
        }

        [HttpPost("add-balance")]
        [Authorize(Policy = "AddBalance")]
        public async Task<IActionResult> AddBalance([FromBody] AddBalanceCommand command)
        {
            var newBalance = await _mediator.Send(command);
            return Ok(new { balance = newBalance });
        }

        [HttpPost("spend-balance")]
        [Authorize(Policy = "SpendBalance")]
        public async Task<IActionResult> SpendBalance([FromBody] SpendBalanceCommand command)
        {
            var newBalance = await _mediator.Send(command);
            return Ok(new { balance = newBalance });
        }
    }
}
