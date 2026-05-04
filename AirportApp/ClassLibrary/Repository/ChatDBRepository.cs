using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.DataAccess;

namespace AirportApp.ClassLibrary.Repository
{
    public class ChatDatabaseRepository : IRepository<int, Chat>
    {
        private readonly AirportDbContext dataBaseContext;

        public ChatDatabaseRepository(AirportDbContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public int CreateNewEntity(Chat incomingChatEntityToBeSaved)
        {
            if (incomingChatEntityToBeSaved == null)
            {
                throw new ArgumentNullException(nameof(incomingChatEntityToBeSaved));
            }

            this.dataBaseContext.chats.Add(incomingChatEntityToBeSaved);
            this.dataBaseContext.SaveChanges();
            return incomingChatEntityToBeSaved.Id;
        }

        public void DeleteById(int identifierForChatToBeDeleted)
        {
            var chat = this.dataBaseContext.chats.FirstOrDefault(c => c.Id == identifierForChatToBeDeleted);
            if (chat != null)
            {
                this.dataBaseContext.chats.Remove(chat);
                this.dataBaseContext.SaveChanges();
            }
        }

        public void UpdateById(int identifierForChatToBeUpdated, Chat updatedChatEntityData)
        {
            if (updatedChatEntityData == null)
            {
                throw new ArgumentNullException(nameof(updatedChatEntityData));
            }

            var chatFound = this.dataBaseContext.chats.FirstOrDefault(c => c.Id == identifierForChatToBeUpdated);
            if (chatFound != null)
            {
                chatFound.Status = updatedChatEntityData.Status;
                this.dataBaseContext.SaveChanges();
            }
        }

        public IEnumerable<Chat> GetAll()
        {
            return this.dataBaseContext.chats.ToList();
        }

        public Chat GetById(int identifierForRequestedChat)
        {
            var chat = this.dataBaseContext.chats.FirstOrDefault(c => c.Id == identifierForRequestedChat);
            return chat ?? throw new KeyNotFoundException($"Chat with id {identifierForRequestedChat} not found.");
        }
    }
}