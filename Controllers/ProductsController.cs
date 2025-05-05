using rahayu_konveksi_api.Models;
using rahayu_konveksi_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace rahayu_konveksi_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsService _productsService;

        public ProductsController(ProductsService productsService)
        {
            _productsService = productsService;
        }

        // GET: api/products
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            var products = await _productsService.GetAllProductsAsync();
            return Ok(products);
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Product>> GetProductById(string id)
        {
            var product = await _productsService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }
            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            await _productsService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] Product productIn)
        {
            var product = await _productsService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            productIn.Id = id;

            await _productsService.UpdateProductAsync(id, productIn);
            return Ok(new { message = "Product updated successfully" });
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            var product = await _productsService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            await _productsService.DeleteProductAsync(id);
            return Ok(new { message = "Product deleted successfully" });
        }

        // GET: api/products/category/{category}
        [HttpGet("category/{category}")]
        [Authorize]
        public async Task<ActionResult<List<Product>>> GetProductsByCategory(string category)
        {
            var products = await _productsService.GetProductsByCategoryAsync(category);
            return Ok(products);
        }
    }
}