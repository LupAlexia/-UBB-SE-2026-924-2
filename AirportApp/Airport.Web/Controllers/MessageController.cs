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
        private readonly IUserService userService;

        public MessageController(IMessageService messageService, IUserService userService)
        {
            this.messageService = messageService;
            this.userService = userService;
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
                return Ok(new MessageDTO
                {
                    MessageId = message.Id,
                    MessageText = message.Text,
                    Timestamp = message.Timestamp,
                    ChatId = message.Chat.Id,
                    SenderId = message.Sender.RetrieveUniqueDatabaseIdentifierForBot(),
                    Sender = message.Sender
                });
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

        [HttpPost("send")]
        public async Task<ActionResult<BotReplyDTO>> SendMessageAsync([FromBody] SendMessageRequestDTO request)
        {
            try
            {
                var sender = await userService.GetByIdAsync(request.SenderId);
                if (sender == null)
                {
                    return NotFound(new { Message = $"User with id {request.SenderId} was not found." });
                }

                var faqOption = new FAQOption
                {
                    Label = request.OptionLabel,
                    NextOption = request.NextNodeId.HasValue ? new FAQNode
                    {
                        NodeId = request.NextNodeId.Value
                    }
                    : null
                };

                BotMessage botReply = await messageService.SendMessageAsync(request.ChatId, sender, faqOption);

                var replyDTO = new BotReplyDTO
                {
                    MessageId = botReply.Id,
                    Text = botReply.Text,
                    Timestamp = botReply.Timestamp,
                    FAQOptions = botReply.FAQOptions?.Select(o => MapFAQOptionToDTO(o)).ToList() ?? new List<FAQOptionDTO>()
                };

                return Ok(replyDTO);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        private static FAQOptionDTO MapFAQOptionToDTO(FAQOption option)
        {
            return new FAQOptionDTO
            {
                OptionId = option.OptionId,
                Label = option.Label,
                NextNodeId = option.NextOption?.NodeId
            };
        }
    }
}