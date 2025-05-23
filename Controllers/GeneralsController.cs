using rahayu_konveksi_api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace rahayu_konveksi_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralsController(GeneralsService generalsService) : ControllerBase
    {
        private readonly GeneralsService _generalsService = generalsService;

        // GET: api/generals
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetGenerals()
        {
            var generals = await _generalsService.GetAllGeneralsAsync();
            return Ok(generals);
        }
    }
}