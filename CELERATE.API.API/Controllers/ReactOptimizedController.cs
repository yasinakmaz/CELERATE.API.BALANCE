using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CELERATE.API.API.Controllers
{
    [ApiController]
    [Route("api/react")]
    [Authorize]
    public class ReactOptimizedController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReactOptimizedController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Özel endpoint: NFC kartı okutulduğunda tüm kart bilgilerini tek seferde döndürür
        [HttpGet("card-scan/{nfcId}")]
        public async Task<IActionResult> GetCardFullInfo(string nfcId)
        {
            var result = await _mediator.Send(new GetCardFullInfoQuery { NfcId = nfcId });

            if (result == null)
            {
                return NotFound(new { message = "Kart bulunamadı" });
            }

            return Ok(result);
        }

        // Özel endpoint: Dashboard için gereken tüm bilgileri tek seferde döndürür
        [HttpGet("dashboard-summary")]
        [Authorize(Policy = "ViewDashboard")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var result = await _mediator.Send(new GetDashboardSummaryQuery());
            return Ok(result);
        }

        // Özel endpoint: Sayfalı müşteri listesi
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomersList(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string searchTerm = null)
        {
            var result = await _mediator.Send(new GetCustomersListQuery
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm
            });

            return Ok(result);
        }

        // Özel endpoint: Müşteri detayları ve tüm işlem geçmişi
        [HttpGet("customer-details/{userId}")]
        public async Task<IActionResult> GetCustomerDetails(string userId)
        {
            var result = await _mediator.Send(new GetCustomerDetailsQuery { UserId = userId });

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // Özel endpoint: React select dropdown için şube listesi
        [HttpGet("branches-dropdown")]
        public async Task<IActionResult> GetBranchesForDropdown()
        {
            var result = await _mediator.Send(new GetBranchesForDropdownQuery());
            return Ok(result);
        }
    }
}
