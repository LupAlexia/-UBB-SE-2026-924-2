using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IRepository<int, Chat> chatRepository;

        public ChatController(IRepository<int, Chat> chatRepository)
        {
            this.chatRepository = chatRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Chat>>> GetAllAsync()
        {
            IEnumerable<Chat> chats = await chatRepository.GetAllAsync();
            return Ok(chats);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Chat>> GetByIdAsync(int id)
        {
            try
            {
                Chat chat = await chatRepository.GetByIdAsync(id);
                return Ok(chat);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] Chat chat)
        {
            int createdId = await chatRepository.CreateNewEntityAsync(chat);
            chat.Id = createdId;
            return CreatedAtAction(nameof(GetByIdAsync), new { id = createdId }, chat);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] Chat chat)
        {
            if (id != chat.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            await chatRepository.UpdateByIdAsync(id, chat);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await chatRepository.DeleteByIdAsync(id);
            return NoContent();
        }
    }
}
