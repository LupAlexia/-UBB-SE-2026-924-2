using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageRepository messageRepository;
        private readonly IRepository<int, Chat> chatRepository;
        private readonly IRepository<int, Sender> senderRepository;

        public MessageController(IMessageRepository messageRepository, IRepository<int, Chat> chatRepository, IRepository<int, Sender> senderRepository)
        {
            this.messageRepository = messageRepository;
            this.chatRepository = chatRepository;
            this.senderRepository = senderRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetAllAsync()
        {
            IEnumerable<Message> messages = await messageRepository.GetAllAsync();
            var dtos = messages.Select(m => new MessageDTO
            {
                MessageId = m.Id,
                MessageText = m.Text,
                Timestamp = m.Timestamp,
                ChatId = m.Chat.Id,
                SenderId = m.Sender.RetrieveUniqueDatabaseIdentifierForBot(),
                Sender = m.Sender
            });
            return Ok(dtos);
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
        public async Task<ActionResult> CreateAsync([FromBody] CreateMessageDTO dto)
        {
            if (dto == null)
            {
                return BadRequest(new { Message = "ChatId and SenderId are required." });
            }
            if (dto.chatId < 0)
            {
                return BadRequest(new { Message = "ChatId and SenderId are required." });
            }
            if (dto.senderId <= -2)
            {
                return BadRequest(new { Message = "ChatId and SenderId are required." });
            }

            try
            {
                // Fetch Chat and Sender from database
                Chat chat = await chatRepository.GetByIdAsync(dto.chatId);
                Sender sender = await senderRepository.GetByIdAsync(dto.senderId);

                if (chat == null || sender == null)
                {
                    return NotFound(new { Message = "Chat or Sender not found." });
                }

                // Reconstruct Message with populated navigations
                var message = new Message(chat, dto.text, sender)
                {
                    Timestamp = dto.timestamp == default ? DateTimeOffset.UtcNow : dto.timestamp
                };

                int createdId = await messageRepository.CreateNewEntityAsync(message);
                return CreatedAtAction(nameof(GetByIdAsync), new { id = createdId }, message);
            }
            catch (KeyNotFoundException kex)
            {
                return NotFound(new { Message = kex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
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
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetByChatIdAsync(int chatId)
        {
            IEnumerable<Message> messages = await messageRepository.GetByChatIdAsync(chatId);
            var dtos = messages.Select(m => new MessageDTO
            {
                MessageId = m.Id,
                MessageText = m.Text,
                Timestamp = m.Timestamp,
                ChatId = m.Chat.Id,
                SenderId = m.Sender.RetrieveUniqueDatabaseIdentifierForBot(),
                Sender = m.Sender
            });
            return Ok(dtos);
        }

        [HttpGet("chat/{chatId}/since/{firstMessageId}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessagesSinceAsync(int chatId, int firstMessageId)
        {
            IEnumerable<Message> messages = await messageRepository.GetMessagesSinceAsync(chatId, firstMessageId);
            return Ok(messages);
        }

        [HttpGet("chat/{chatId}/with-senders")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetByChatIdWithSendersAsync(int chatId)
        {
            var messages = await messageRepository.GetByChatIdAsync(chatId);
            var result = messages.Select(m => new MessageDTO
            {
                MessageId = m.Id,
                MessageText = m.Text,
                Timestamp = m.Timestamp,
                ChatId = m.Chat.Id,
                Sender = m.Sender
            });
            return Ok(result);
        }
    }
}
