using rahayu_konveksi_api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace rahayu_konveksi_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController(LoansService loansService) : ControllerBase
    {
        private readonly LoansService _loansService = loansService;

        // GET: api/loans
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLoans()
        {
            var loans = await _loansService.GetAllLoansAsync();
            return Ok(loans);
        }
    }
}