using rahayu_konveksi_api.Models;
using rahayu_konveksi_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Minio;
using Minio.Exceptions;
using System.IO;
using Minio.DataModel.Args;

namespace rahayu_konveksi_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController(EmployeesService employeesService, IMinioClient minioClient) : ControllerBase
    {
        private readonly EmployeesService _employeesService = employeesService;
        private readonly IMinioClient _minioClient = minioClient;

        // GET: api/employees
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<Employee>>> GetAllEmployees()
        {
            var employees = await _employeesService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        // GET: api/employees/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Employee>> GetEmployeeById(string id)
        {
            var employee = await _employeesService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = "Employee not found" });
            }
            return Ok(employee);
        }

        // POST: api/employees
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Employee>> CreateEmployee([FromForm] Employee employee, IFormFile image)
        {
            if (image != null)
            {
                var bucketName = "rahayu-konveksi";
                var objectName = $"{employee.Name.Replace(" ", "-")}.jpg";
                var filePath = Path.Combine(Path.GetTempPath(), objectName);

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await image.CopyToAsync(stream);
                }
                
                try
                {
                    var putObjectArgs = new PutObjectArgs()
                            .WithBucket(bucketName)
                            .WithObject(objectName)
                            .WithFileName(filePath)
                            .WithContentType("application/octet-stream");

                    await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
                    employee.Photo = $"https://minio-q00wcwgsscsgk8k8socss0ws.34.126.166.246.sslip.io/{bucketName}/{objectName}";
                    await _employeesService.CreateEmployeeAsync(employee);
                    return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
                }
                catch (MinioException ex)
                {
                    return BadRequest(new { message = $"Error uploading photo: {ex.Message}" });
                }
            }
            else
            {
                return BadRequest(new { message = "Photo is required" });
            }
        }

        // PUT: api/employees/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateEmployee(string id, [FromForm] Employee employeeIn, IFormFile image)
        {
            var employee = await _employeesService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = "Employee not found" });
            }

            if (image != null)
            {
                var bucketName = "rahayu-konveksi";
                var objectName = $"{employeeIn.Name.Replace(" ", "-")}.jpg";
                var filePath = Path.Combine(Path.GetTempPath(), objectName);

                using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await image.CopyToAsync(stream);
                }

                try
                {
                    var putObjectArgs = new PutObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName)
                        .WithFileName(filePath)
                        .WithContentType("application/octet-stream");

                    await _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

                    // Set the public URL of the uploaded image
                    employeeIn.Photo = $"https://minio-q00wcwgsscsgk8k8socss0ws.34.126.166.246.sslip.io/{bucketName}/{objectName}";
                }
                catch (MinioException ex)
                {
                    return BadRequest(new { message = $"Error uploading photo: {ex.Message}" });
                }
            }
            else
            {
                employeeIn.Photo = employee.Photo; // Retain the existing photo if none is provided
            }

            employeeIn.Id = id;

            await _employeesService.UpdateEmployeeAsync(id, employeeIn);
            return Ok(new { message = "Employee updated successfully" });
        }

        // DELETE: api/employees/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteEmployee(string id)
        {
            var employee = await _employeesService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = "Employee not found" });
            }

            if (!string.IsNullOrEmpty(employee.Photo))
            {
                var bucketName = "rahayu-konveksi";
                var objectName = employee.Photo.Substring(employee.Photo.LastIndexOf('/') + 1);

                try
                {
                    var removeObjectArgs = new RemoveObjectArgs()
                        .WithBucket(bucketName)
                        .WithObject(objectName);

                    await _minioClient.RemoveObjectAsync(removeObjectArgs);
                }
                catch (MinioException ex)
                {
                    return BadRequest(new { message = $"Error deleting photo: {ex.Message}" });
                }
            }

            await _employeesService.DeleteEmployeeAsync(id);
            return Ok(new { message = "Employee deleted successfully" });
        }
    }
}