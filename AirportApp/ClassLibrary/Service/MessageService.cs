using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Service.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.Src.Service
{
    public class MessageService : IMessageService
    {
        private readonly IRepository<int, Chat> chatRepository;
        private readonly IMessageRepository messageRepository;
        private readonly BotEngineIdentity botEngine;

        public MessageService(
            IRepository<int, Chat> chatRepository,
            IMessageRepository messageRepository,
            BotEngineIdentity botEngine)
        {
            this.chatRepository = chatRepository ?? throw new ArgumentNullException(nameof(chatRepository));
            this.messageRepository = messageRepository ?? throw new ArgumentNullException(nameof(messageRepository));
            this.botEngine = botEngine ?? throw new ArgumentNullException(nameof(botEngine));
        }

        public async Task<BotMessage> SendMessageAsync(int chatId, Sender sender, FAQOption selectedOption)
        {
            if (selectedOption == null)
            {
                throw new ArgumentNullException(nameof(selectedOption));
            }
            if (selectedOption.NextOption?.NodeId == 1)
            {
                await botEngine.ResetBotConversationStateToInitialRootNodeAsync();
            }

            Chat chat = await GetActiveChatAsync(chatId);

            var userMessage = new Message(chat, selectedOption.Label, sender);
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
            var allMessages = await messageRepository.GetAllAsync();
            return allMessages
                .Where(chatMessage => chatMessage.Chat.Id == chatId)
                .OrderBy(chatMessage => chatMessage.Timestamp);
        }

        public async Task<IEnumerable<Message>> GetAllAsync()
        {
            return await messageRepository.GetAllAsync();
        }

        public async Task<Message> GetByIdAsync(int id)
        {
            return await messageRepository.GetByIdAsync(id);
        }

        public async Task<int> CreateMessageAsync(int chatId, int senderId, string text, DateTimeOffset timestamp)
        {
            Chat chat = await chatRepository.GetByIdAsync(chatId);
            Sender sender = await messageRepository.GetSenderByIdAsync(senderId);

            var message = new Message(chat, text, sender)
            {
                Timestamp = timestamp == default ? DateTimeOffset.UtcNow : timestamp
            };

            return await messageRepository.CreateNewEntityAsync(message);
        }

        public async Task UpdateByIdAsync(int id, Message message)
        {
            await messageRepository.UpdateByIdAsync(id, message);
        }

        public async Task DeleteByIdAsync(int id)
        {
            await messageRepository.DeleteByIdAsync(id);
        }

        public async Task<IEnumerable<Message>> GetByChatIdAsync(int chatId)
        {
            return await messageRepository.GetByChatIdAsync(chatId);
        }

        public async Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int firstMessageId)
        {
            return await messageRepository.GetMessagesSinceAsync(chatId, firstMessageId);
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