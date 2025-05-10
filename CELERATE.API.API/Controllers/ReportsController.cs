using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CELERATE.API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("customers")]
        [Authorize(Policy = "ViewReports")]
        public async Task<IActionResult> GetCustomerReport()
        {
            // Tüm müşterileri getir
            var customers = await _mediator.Send(new GetAllCustomersQuery());

            return Ok(customers);
        }

        [HttpGet("branches")]
        [Authorize(Policy = "ViewReports")]
        public async Task<IActionResult> GetBranchReport()
        {
            // Şube listesini al
            var branches = await _mediator.Send(new GetAllBranchesQuery());

            var branchReport = new List<BranchReportDto>();

            foreach (var branch in branches)
            {
                // Şubenin işlemlerini al
                var transactions = await _mediator.Send(new GetTransactionsByBranchIdQuery { BranchId = branch.Id });

                var addBalanceTotal = transactions
                    .Where(t => t.Type == TransactionType.AddBalance.ToString())
                    .Sum(t => t.Amount);

                var spendBalanceTotal = transactions
                    .Where(t => t.Type == TransactionType.SpendBalance.ToString())
                    .Sum(t => t.Amount);

                branchReport.Add(new BranchReportDto
                {
                    BranchId = branch.Id,
                    BranchName = branch.Name,
                    AddBalanceTotal = addBalanceTotal,
                    SpendBalanceTotal = spendBalanceTotal,
                    Total = addBalanceTotal - spendBalanceTotal
                });
            }

            return Ok(branchReport);
        }

        [HttpGet("staff")]
        [Authorize(Policy = "ViewReports")]
        public async Task<IActionResult> GetStaffReport()
        {
            // Personel listesini al (müşteri olmayanlar)
            var staff = await _mediator.Send(new GetAllStaffQuery());

            var staffReport = new List<StaffReportDto>();

            foreach (var employee in staff)
            {
                // Personelin işlemlerini al
                var transactions = await _mediator.Send(new GetTransactionsByOperatorIdQuery { OperatorId = employee.Id });

                var addBalanceTotal = transactions
                    .Where(t => t.Type == TransactionType.AddBalance.ToString())
                    .Sum(t => t.Amount);

                var spendBalanceTotal = transactions
                    .Where(t => t.Type == TransactionType.SpendBalance.ToString())
                    .Sum(t => t.Amount);

                staffReport.Add(new StaffReportDto
                {
                    StaffId = employee.Id,
                    FullName = employee.FullName,
                    PhoneNumber = employee.PhoneNumber,
                    Gender = employee.Gender,
                    BranchId = employee.BranchId,
                    BranchName = employee.BranchName,
                    AddBalanceTotal = addBalanceTotal,
                    SpendBalanceTotal = spendBalanceTotal,
                    Total = addBalanceTotal - spendBalanceTotal
                });
            }

            return Ok(staffReport);
        }
    }
}
