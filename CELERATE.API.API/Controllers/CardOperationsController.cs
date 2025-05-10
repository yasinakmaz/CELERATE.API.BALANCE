using CELERATE.API.API.Hubs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CELERATE.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CardOperationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<NotificationHub> _hubContext;

        public CardOperationsController(
            IMediator mediator,
            IHubContext<NotificationHub> hubContext)
        {
            _mediator = mediator;
            _hubContext = hubContext;
        }

        // Hızlı bakiye sorgulaması için optimize edilmiş endpoint
        [HttpGet("balance/{nfcId}")]
        public async Task<IActionResult> GetCardBalance(string nfcId)
        {
            var card = await _mediator.Send(new GetCardBalanceQuery { NfcId = nfcId });

            if (card == null)
            {
                return NotFound();
            }

            // Sadece gerekli alanları döndür
            return Ok(new { balance = card.Balance });
        }

        // Optimized bulk operations
        [HttpPost("bulk-add-balance")]
        [Authorize(Policy = "AddBalance")]
        public async Task<IActionResult> BulkAddBalance([FromBody] BulkAddBalanceCommand command)
        {
            var results = await _mediator.Send(command);

            // Realtime bildirim gönder
            foreach (var result in results)
            {
                await _hubContext.Clients
                    .Group($"User_{result.UserId}")
                    .SendAsync("BalanceChanged", result.NewBalance);

                await _hubContext.Clients
                    .Group($"Branch_{result.BranchId}")
                    .SendAsync("NewTransaction", new { userId = result.UserId, amount = result.Amount });
            }

            return Ok(results);
        }
        [HttpPost("quick-auth")]
        public async Task<IActionResult> QuickAuth([FromBody] QuickAuthCommand command)
        {
            var result = await _mediator.Send(command);

            if (!result.Succeeded)
            {
                return Unauthorized(new { message = result.ErrorMessage });
            }

            // Minimal yanıt, sadece gerekli bilgiler
            return Ok(new
            {
                token = result.Token,
                permissions = result.Permissions
            });
        }
    }
}

