using rahayu_konveksi_api.Models;
using rahayu_konveksi_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace rahayu_konveksi_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController(EmployeesService employeesService) : ControllerBase
    {
        private readonly EmployeesService _employeesService = employeesService;

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
        public async Task<ActionResult<Employee>> CreateEmployee([FromBody] Employee employee)
        {
            await _employeesService.CreateEmployeeAsync(employee);
            return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee);
        }

        // PUT: api/employees/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateEmployee(string id, [FromBody] Employee employeeIn)
        {
            var employee = await _employeesService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound(new { message = "Employee not found" });
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
            await _employeesService.DeleteEmployeeAsync(id);
            return Ok(new { message = "Employee deleted successfully" });
        }

        // GET: api/employees/status/{status}
        [HttpGet("status/{status}")]
        [Authorize]
        public async Task<ActionResult<List<Employee>>> GetEmployeesByStatus(string status)
        {
            var employees = await _employeesService.GetEmployeesByStatusAsync(status);
            return Ok(employees);
        }

        // GET: api/employees/position/{position}
        [HttpGet("position/{position}")]
        [Authorize]
        public async Task<ActionResult<List<Employee>>> GetEmployeesByPosition(string position)
        {
            var employees = await _employeesService.GetEmployeesByPositionAsync(position);
            return Ok(employees);
        }
    }
}