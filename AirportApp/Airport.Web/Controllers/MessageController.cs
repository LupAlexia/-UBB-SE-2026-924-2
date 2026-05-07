using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Repository.Interfaces;

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
            if (id != message.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

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

        [HttpGet("chat/{chatId}/with-senders")]
        public async Task<ActionResult<IEnumerable<MessageWithSenderDTO>>> GetByChatIdWithSendersAsync(int chatId)
        {
            var messages = await messageRepository.GetByChatIdAsync(chatId);
            var result = messages.Select(m => new MessageWithSenderDTO
            {
                Id = m.Id,
                Text = m.Text,
                Timestamp = m.Timestamp,
                ChatId = m.ChatId,
                SenderUserId = m.SenderUserId,
                SenderEmployeeId = m.SenderEmployeeId
            });
            return Ok(result);
        }
    }
}
