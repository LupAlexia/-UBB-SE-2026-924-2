using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllAsync()
        {
            IEnumerable<User> users = await userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetByIdAsync(int id)
        {
            try
            {
                User user = await userService.GetByIdAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] User user)
        {
            int createdId = await userService.AddUserAsync(user);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = createdId }, user);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] User user)
        {
            if (id != user.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            await userService.UpdateUserByIdAsync(id, user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await userService.DeleteUserByIdAsync(id);
            return NoContent();
        }
    }
}