using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.Src.Service.Interfaces;

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

        public async Task<Chat> OpenChatAsync(int userId)
        {
            User user = await userRepository.GetByIdAsync(userId);
            try
            {
                Chat newChat = new Chat(UNASSIGNED_CHAT_ID, user, ChatStatus.Active);
                int newIdentificationNumber = Convert.ToInt32(await chatRepository.CreateNewEntityAsync(newChat));
                newChat.Id = newIdentificationNumber;
                return newChat;
            }
            catch (Exception exceptionThrown)
            {
                throw (new Exception(message: exceptionThrown.Message));
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
    }
}
