using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Service
{
    public class ChatService
    {
        private IRepository<int, Chat> chatRepository;
        public const int UNASSIGNED_CHAT_ID = 0;

        public ChatService(IRepository<int, Chat> chatRepository)
        {
            this.chatRepository = chatRepository;
        }

        public Chat OpenChat(int userId)
        {
            try
            {
                Chat newChat = new Chat(UNASSIGNED_CHAT_ID, userId, ChatStatus.Active);
                int newIdentificationNumber = Convert.ToInt32(chatRepository.CreateNewEntity(newChat));
                newChat.ChatId = newIdentificationNumber;
                return newChat;
            }
            catch (Exception exceptionThrown)
            {
                    throw (new Exception(message: exceptionThrown.Message));
            }
        }

        public void CloseChat(int chatId)
        {
            try
            {
                Chat chat = chatRepository.GetById(chatId);
                chat.CloseChat();
                chatRepository.UpdateById(chatId, chat);
            }
            catch (Exception exceptionThrown)
            {
                throw new Exception(exceptionThrown.Message);
            }
        }
    }
}
