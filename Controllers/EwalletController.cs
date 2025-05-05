using rahayu_konveksi_api.Models;
using rahayu_konveksi_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace rahayu_konveksi_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EwalletController(EwalletService ewalletService) : ControllerBase
    {
        private readonly EwalletService _ewalletService = ewalletService;

        // GET: api/ewallet/balance
        // This endpoint retrieves the balance of the ewallet.
        [HttpGet("balance")]
        public async Task<IActionResult> GetEwalletBalance()
        {
            var ewallet = await _ewalletService.GetEwalletBalanceAsync();
            if (ewallet == null)
            {
                return NotFound(new { message = "Ewallet not found" });
            }
            return Ok(new { balance = ewallet });
        }
    }
}