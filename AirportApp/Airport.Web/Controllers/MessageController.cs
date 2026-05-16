using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AirportApp.ClassLibrary.Entity.Dto;
using AirportApp.ClassLibrary.Service.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain;

namespace Airport.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService messageService;

        public MessageController(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetAllAsync()
        {
            IEnumerable<Message> messages = await messageService.GetAllAsync();
            var dtos = messages.Select(messageEntity => new MessageDTO
            {
                MessageId = messageEntity.Id,
                MessageText = messageEntity.Text,
                Timestamp = messageEntity.Timestamp,
                ChatId = messageEntity.Chat.Id,
                SenderId = messageEntity.Sender.RetrieveUniqueDatabaseIdentifierForBot(),
                Sender = messageEntity.Sender
            });
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Message>> GetByIdAsync(int id)
        {
            try
            {
                Message message = await messageService.GetByIdAsync(id);
                return Ok(message);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([FromBody] CreateMessageDTO messageCreationData)
        {
            if (messageCreationData == null)
            {
                return BadRequest(new { Message = "ChatId and SenderId are required." });
            }
            if (messageCreationData.chatId < 0)
            {
                return BadRequest(new { Message = "ChatId and SenderId are required." });
            }
            if (messageCreationData.senderId <= -2)
            {
                return BadRequest(new { Message = "ChatId and SenderId are required." });
            }

            try
            {
                int createdId = await messageService.CreateMessageAsync(
                    messageCreationData.chatId,
                    messageCreationData.senderId,
                    messageCreationData.text,
                    messageCreationData.timestamp);

                return CreatedAtAction(nameof(GetByIdAsync), new { id = createdId }, null);
            }
            catch (KeyNotFoundException keyNotFoundException)
            {
                return NotFound(new { Message = keyNotFoundException.Message });
            }
            catch (Exception exception)
            {
                return BadRequest(new { Message = exception.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] Message message)
        {
            if (id != message.Id)
            {
                return BadRequest("ID in URL does not match ID in body.");
            }

            await messageService.UpdateByIdAsync(id, message);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            await messageService.DeleteByIdAsync(id);
            return NoContent();
        }

        [HttpGet("chat/{chatId}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetByChatIdAsync(int chatId)
        {
            IEnumerable<Message> messages = await messageService.GetByChatIdAsync(chatId);
            var dtos = messages.Select(messageEntity => new MessageDTO
            {
                MessageId = messageEntity.Id,
                MessageText = messageEntity.Text,
                Timestamp = messageEntity.Timestamp,
                ChatId = messageEntity.Chat.Id,
                SenderId = messageEntity.Sender.RetrieveUniqueDatabaseIdentifierForBot(),
                Sender = messageEntity.Sender
            });
            return Ok(dtos);
        }

        [HttpGet("chat/{chatId}/since/{firstMessageId}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessagesSinceAsync(int chatId, int firstMessageId)
        {
            IEnumerable<Message> messages = await messageService.GetMessagesSinceAsync(chatId, firstMessageId);
            return Ok(messages);
        }

        [HttpGet("chat/{chatId}/with-senders")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetByChatIdWithSendersAsync(int chatId)
        {
            var messages = await messageService.GetByChatIdAsync(chatId);
            var result = messages.Select(messageEntity => new MessageDTO
            {
                MessageId = messageEntity.Id,
                MessageText = messageEntity.Text,
                Timestamp = messageEntity.Timestamp,
                ChatId = messageEntity.Chat.Id,
                Sender = messageEntity.Sender
            });
            return Ok(result);
        }
    }
}