using rahayu_konveksi_api.Models;
using rahayu_konveksi_api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace rahayu_konveksi_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EwalletController(EwalletService ewalletService, GeneralsService generalsService) : ControllerBase
    {
        private readonly EwalletService _ewalletService = ewalletService;
        private readonly GeneralsService _generalsService = generalsService;

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
        public async Task<ActionResult<General>> CreatePayoutForGenerals([FromBody]General general)
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
    }
}