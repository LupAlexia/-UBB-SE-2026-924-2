using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Service
{
    public class MessageService : IMessageService
    {
        private readonly IRepository<int, Chat> chatRepository;
        private readonly IRepository<int, Message> messageRepository;
        private readonly BotEngineIdentity botEngine;

        public MessageService(
            IRepository<int, Chat> chatRepository,
            IRepository<int, Message> messageRepository,
            BotEngineIdentity botEngine)
        {
            this.chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
            this.messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
            this.botEngine = botEngine ?? throw new ArgumentNullException(nameof(botEngine));
        }

        public async Task<BotMessage> SendMessageAsync(int chatId, ISender sender, FAQOption selectedOption)
        {
            if (selectedOption == null)
            {
                throw new ArgumentNullException(nameof(selectedOption));
            }
            if (selectedOption.nextOptionId == 1)
            {
                await botEngine.ResetBotConversationStateToInitialRootNodeAsync();
            }

            Chat chat = await GetActiveChatAsync(chatId);

            var userMessage = new Message(chat, selectedOption.label, sender);
            await messageRepository.CreateNewEntityAsync(userMessage);

            BotMessage botReply = await botEngine.GenerateAppropriateResponseBasedOnCurrentStrategyAsync(userMessage);

            var botRow = new Message(chat, botReply.GetMessage(), botEngine);
            await messageRepository.CreateNewEntityAsync(botRow);

            return botReply;
        }

        public async Task<IMessage> GetMessageAsync(int chatId, int messageId)
        {
            IMessage message = await messageRepository.GetByIdAsync(messageId);
            if (message.GetChat().Id != chatId)
            {
                throw new InvalidOperationException($"Message {messageId} does not belong to chat {chatId}.");
            }
            return message;
        }

        public async Task<IEnumerable<Message>> GetAllMessagesAsync(int chatId)
        {
            _ = await chatRepository.GetByIdAsync(chatId);

            var allMessages = await messageRepository.GetAllAsync();
            return allMessages
                .Where(chatMessage => chatMessage.ChatId == chatId)
                .OrderBy(chatMessage => chatMessage.Timestamp);
        }

        private async Task<Chat> GetActiveChatAsync(int chatId)
        {
            Chat chat = await chatRepository.GetByIdAsync(chatId);
            if (chat.Status != ChatStatus.Active)
            {
                throw new InvalidOperationException($"Chat {chatId} is not active.");
            }
            return chat;
        }
    }
}