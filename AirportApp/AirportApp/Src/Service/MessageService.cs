using System;
using System.Collections.Generic;
using System.Linq;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Service
{
    public class MessageService
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

        public BotMessage SendMessage(int chatId, ISender sender, FAQOption selectedOption)
        {
            if (selectedOption == null)
            {
                throw new ArgumentNullException(nameof(selectedOption));
            }
            if (selectedOption.NextOptionId == 1)
            {
                botEngine.ResetBotConversationStateToInitialRootNode();
            }

            Chat chat = GetActiveChat(chatId);

            var userMessage = new Message(chat, selectedOption.Label, sender);
            messageRepository.CreateNewEntity(userMessage);

            BotMessage botReply = botEngine.GenerateAppropriateResponseBasedOnCurrentStrategy(userMessage);

            var botRow = new Message( chat, botReply.GetMessage(), botEngine);
            messageRepository.CreateNewEntity(botRow);

            return botReply;
        }

        public IMessage GetMessage(int chatId, int messageId)
        {
            IMessage message = messageRepository.GetById(messageId);
            if (message.GetChat().Id != chatId)
            {
                throw new InvalidOperationException($"Message {messageId} does not belong to chat {chatId}.");
            }
            return message;
        }

        public IEnumerable<Message> GetAllMessages(int chatId)
        {
            _ = chatRepository.GetById(chatId);

            return messageRepository.GetAll()
                .Where(chatMessage => chatMessage.GetChat().Id == chatId)
                .OrderBy(chatMessage => ((IMessage)chatMessage).Timestamp);
        }

        private Chat GetActiveChat(int chatId)
        {
            Chat chat = chatRepository.GetById(chatId);
            if (chat.Status != ChatStatus.Active)
            {
                throw new InvalidOperationException($"Chat {chatId} is not active.");
            }
            return chat;
        }
    }
}