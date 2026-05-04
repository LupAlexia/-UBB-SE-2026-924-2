using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository messageRepository;

        public MessageController(IMessageRepository messageRepository)
        {
            this.messageRepository = messageRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Message>>> GetAllAsync()
        {
            IEnumerable<Message> messages = await messageRepository.GetAllAsync();
            return Ok(messages);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetByIdAsync(int id)
        {
            try
            {
                Message message = await messageRepository.GetByIdAsync(id);
                return Ok(message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] Message message)
        {
            int createdId = await messageRepository.CreateNewEntityAsync(message);
            return CreatedAtAction(nameof(GetByIdAsync), new { id = createdId }, message);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] Message message)
        {
            await messageRepository.UpdateByIdAsync(id, message);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await messageRepository.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpGet("chat/{chatId}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetByChatIdAsync(int chatId)
        {
            IEnumerable<Message> messages = await messageRepository.GetByChatIdAsync(chatId);
            return Ok(messages);
        }

        [HttpGet("chat/{chatId}/since/{firstMessageId}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessagesSinceAsync(int chatId, int firstMessageId)
        {
            IEnumerable<Message> messages = await messageRepository.GetMessagesSinceAsync(chatId, firstMessageId);
            return Ok(messages);
        }
    }
}
