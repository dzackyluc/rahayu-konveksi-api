using rahayu_konveksi_api.Models;
using rahayu_konveksi_api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace rahayu_konveksi_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EwalletController(EwalletService ewalletService, GeneralsService generalsService, LoansService loansService, PayrollsService payrollsService, OrdersService ordersService) : ControllerBase
    {
        private readonly EwalletService _ewalletService = ewalletService;
        private readonly GeneralsService _generalsService = generalsService;
        private readonly LoansService _loansService = loansService;
        private readonly PayrollsService _payrollsService = payrollsService;
        private readonly OrdersService _ordersService = ordersService;

        // GET: api/ewallet/balance
        // This endpoint retrieves the balance of the ewallet.
        [HttpGet("balance")]
        [Authorize]
        public async Task<IActionResult> GetEwalletBalance()
        {
            var ewallet = await _ewalletService.GetEwalletBalanceAsync();
            if (ewallet == null)
            {
                return NotFound(new { message = "Cannot connect to xendit service" });
            }
            return Ok(new { balance = ewallet });
        }

        // POST: api/ewallet/transactions
        [HttpGet("transactions")]
        [Authorize]
        public async Task<IActionResult> GetEwalletTransactions()
        {
            var transactions = await _ewalletService.GetEwalletTransactionsAsync();
            if (transactions == null)
            {
                return NotFound(new { message = "Cannot connect to xendit service" });
            }
            return Ok(transactions);
        }

        // POST: api/ewallet/payout/generals
        [HttpPost("payout/generals")]
        [Authorize]
        public async Task<ActionResult<General>> CreatePayoutForGenerals([FromBody] General general)
        {
            if (general == null)
            {
                return BadRequest(new { message = "Invalid request" });
            }

            var payoutResponse = await _ewalletService.CreatePayoutAsync($"general-{Encoding.ASCII.GetBytes(general.Name)}", general.TotalExpense, general.Email);

            if (payoutResponse == null)
            {
                return NotFound(new { message = "Cannot connect to xendit service" });
            }
            general.Status = payoutResponse.Value.GetProperty("status").GetString() ?? string.Empty;
            general.XenditRef = payoutResponse.Value.GetProperty("id").GetString() ?? string.Empty;

            // Save the general to the database
            await _generalsService.CreateGeneralAsync(general);
            return CreatedAtAction(nameof(GetEwalletTransactions), new { id = general.Id }, general);
        }

        // POST: api/ewallet/payout/loans
        [HttpPost("payout/loans")]
        [Authorize]
        public async Task<ActionResult<Loan>> CreatePayoutForLoans([FromBody] Loan loan, string Email)
        {
            if (loan == null)
            {
                return BadRequest(new { message = "Invalid request" });
            }

            if (string.IsNullOrEmpty(Email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            if (loan.Transaction == "Transfer")
            {
                var payoutResponse = await _ewalletService.CreatePayoutAsync($"loan-{Encoding.ASCII.GetBytes(loan.EmployeeId)}", loan.Amount, Email);
                if (payoutResponse == null)
                {
                    return NotFound(new { message = "Cannot connect to xendit service" });
                }
                loan.Status = payoutResponse.Value.GetProperty("status").GetString() ?? string.Empty;
                loan.XenditRef = payoutResponse.Value.GetProperty("id").GetString() ?? string.Empty;
            }


            // Save the loan to the database
            await _loansService.CreateLoanAsync(loan);
            return CreatedAtAction(nameof(GetEwalletTransactions), new { id = loan.Id }, loan);
        }

        // POST: api/ewallet/payout/payrolls
        [HttpPost("payout/payrolls")]
        [Authorize]
        public async Task<ActionResult<Payroll>> CreatePayoutForPayrolls([FromBody] Payroll payroll, string Email)
        {
            if (payroll == null)
            {
                return BadRequest(new { message = "Invalid request" });
            }

            if (string.IsNullOrEmpty(Email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            if (payroll.Transaction == "Transfer")
            {
                var payoutResponse = await _ewalletService.CreatePayoutAsync($"payroll-{Encoding.ASCII.GetBytes(payroll.EmployeeId)}", payroll.SalaryPaid, Email);
                if (payoutResponse == null)
                {
                    return NotFound(new { message = "Cannot connect to xendit service" });
                }
                payroll.Status = payoutResponse.Value.GetProperty("status").GetString() ?? string.Empty;
                payroll.XenditRef = payoutResponse.Value.GetProperty("id").GetString() ?? string.Empty;
            }

            // Save the payroll to the database
            await _payrollsService.CreatePayrollAsync(payroll);
            return CreatedAtAction(nameof(GetEwalletTransactions), new { id = payroll.Id }, payroll);
        }

        // POST: api/ewallet/payment/orders
        [HttpPost("payment/orders")]
        [Authorize]
        public async Task<ActionResult<Order>> CreatePaymentForOrders([FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest(new { message = "Invalid request" });
            }

            var payoutResponse = await _ewalletService.CreatePaymentAsync($"order-{Encoding.ASCII.GetBytes(order.CustomerName)}", order.TotalPrice, order.Extras);
            if (payoutResponse == null)
            {
                return NotFound(new { message = "Cannot connect to xendit service" });
            }
            order.Status = payoutResponse.Value.GetProperty("status").GetString() ?? string.Empty;
            order.XenditRef = payoutResponse.Value.GetProperty("id").GetString() ?? string.Empty;
            order.PaymentUrl = payoutResponse.Value.GetProperty("invoice_url").GetString() ?? string.Empty;

            // Save the order to the database
            await _ordersService.CreateOrderAsync(order);
            return CreatedAtAction(nameof(GetEwalletTransactions), new { id = order.Id }, order);
        }
    }
}