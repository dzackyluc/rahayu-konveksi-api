using rahayu_konveksi_api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace rahayu_konveksi_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(OrdersService ordersService) : ControllerBase
    {
        private readonly OrdersService _ordersService = ordersService;

        // GET: api/orders
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _ordersService.GetAllOrdersAsync();
            return Ok(orders);
        }
    }
}