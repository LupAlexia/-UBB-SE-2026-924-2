using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Employee;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepository employeeRepository;

        public EmployeeController(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAllAsync()
        {
            IEnumerable<Employee> employees = await employeeRepository.GetAllAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetByIdAsync(int id)
        {
            try
            {
                Employee employee = await employeeRepository.GetByIdAsync(id);
                return Ok(employee);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] Employee employee)
        {
            int createdId = await employeeRepository.CreateNewEntityAsync(employee);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = createdId }, employee);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] Employee employee)
        {
            await employeeRepository.UpdateByIdAsync(id, employee);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await employeeRepository.DeleteByIdAsync(id);
            return NoContent();
        }
    }
}
