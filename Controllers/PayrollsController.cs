using rahayu_konveksi_api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace rahayu_konveksi_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayrollsController(PayrollsService payrollsService) : ControllerBase
    {
        private readonly PayrollsService _payrollsService = payrollsService;

        // GET: api/payrolls
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPayrolls()
        {
            var payrolls = await _payrollsService.GetAllPayrollsAsync();
            return Ok(payrolls);
        }
    }
}