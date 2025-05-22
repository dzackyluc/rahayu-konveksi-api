using rahayu_konveksi_api.Models;
using rahayu_konveksi_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace rahayu_konveksi_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(ProductsService productsService, IMinioClient minioClient) : ControllerBase
    {
        private readonly ProductsService _productsService = productsService;
        private readonly IMinioClient _minioClient = minioClient;

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
        public async Task<ActionResult<Product>> CreateProduct([FromForm] Product product, IFormFile image)
        {
            if (image != null)
            {
                var bucketName = "rahayu-konveksi";
                var objectName = $"{product.Name.Replace(" ", "-")}.jpg";
                var filePath = Path.Combine(Path.GetTempPath(), objectName);

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await image.CopyToAsync(stream);
                }

                // Upload the image to MinIO
                try
                {
                    var putObjectArgs = new PutObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                        .WithFileName(filePath)
                        .WithContentType("image/jpeg");

                    await _minioClient.PutObjectAsync(putObjectArgs);
                    product.Photo = $"https://minio-q00wcwgsscsgk8k8socss0ws.34.126.166.246.sslip.io/{bucketName}/{objectName}";
                    await _productsService.CreateProductAsync(product);
                    return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
                }
                catch (MinioException ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error uploading image", error = ex.Message });
                }
            }
            else
            {
                return BadRequest(new { message = "Image is required" });
            }
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProduct(string id, [FromForm] Product productIn, IFormFile image)
        {
            var product = await _productsService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found" });
            }

            if (image != null)
            {
                var bucketName = "rahayu-konveksi";
                var objectName = $"{productIn.Name.Replace(" ", "-")}.jpg";
                var filePath = Path.Combine(Path.GetTempPath(), objectName);

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await image.CopyToAsync(stream);
                }

                // Upload the image to MinIO
                try
                {
                    var putObjectArgs = new PutObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                        .WithFileName(filePath)
                        .WithContentType("application/octet-stream");

                    await _minioClient.PutObjectAsync(putObjectArgs);
                    productIn.Photo = $"https://minio-q00wcwgsscsgk8k8socss0ws.34.126.166.246.sslip.io/{bucketName}/{objectName}";
                }
                catch (MinioException ex)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error uploading image", error = ex.Message });
                }
            }
            else
            {
                productIn.Photo = product.Photo; // Keep the existing photo if no new image is provided
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