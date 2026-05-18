using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.Src.Service
{
    public class ChatService : IChatService
    {
        private IRepository<int, Chat> chatRepository;
        private readonly IRepository<int, User> userRepository;
        public const int UNASSIGNED_CHAT_ID = 0;

        public ChatService(IRepository<int, Chat> chatRepository, IRepository<int, User> userRepository)
        {
            this.chatRepository = chatRepository;
            this.userRepository = userRepository;
        }

        public async Task<Chat> OpenChatAsync(User userToOpenChatFor)
        {
            try
            {
                Chat newChat = new Chat(UNASSIGNED_CHAT_ID, userToOpenChatFor, ChatStatus.Active);
                int newIdentificationNumber = Convert.ToInt32(await chatRepository.CreateNewEntityAsync(newChat));
                newChat.Id = newIdentificationNumber;
                return newChat;
            }
            catch (Exception exceptionThrown)
            {
                throw new Exception(exceptionThrown.Message);
            }
        }

        public async Task CloseChatAsync(int chatId)
        {
            try
            {
                Chat chat = await chatRepository.GetByIdAsync(chatId);
                chat.CloseChat();
                await chatRepository.UpdateByIdAsync(chatId, chat);
            }
            catch (Exception exceptionThrown)
            {
                throw new Exception(exceptionThrown.Message);
            }
        }

        public async Task<IEnumerable<Chat>> GetAllChatsAsync()
        {
            try
            {
                return await chatRepository.GetAllAsync();
            }
            catch (Exception exceptionThrown)
            {
                throw new Exception(exceptionThrown.Message);
            }
        }

        public async Task<Chat> GetChatByIdAsync(int id)
        {
            try
            {
                return await chatRepository.GetByIdAsync(id);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception exceptionThrown)
            {
                throw new Exception(exceptionThrown.Message);
            }
        }

        public async Task UpdateChatAsync(int id, Chat chat)
        {
            try
            {
                await chatRepository.UpdateByIdAsync(id, chat);
            }
            catch (Exception exceptionThrown)
            {
                throw new Exception(exceptionThrown.Message);
            }
        }
    }
}