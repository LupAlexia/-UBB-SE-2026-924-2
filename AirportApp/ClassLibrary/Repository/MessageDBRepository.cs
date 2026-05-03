using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Entity.Domain.Chats;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.DataAccess;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public class MessageDatabaseRepository : IRepository<int, Message>
    {
        private readonly AirportDbContext dataBaseContext;

        public MessageDatabaseRepository(AirportDbContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public int CreateNewEntity(Message newEntity)
        {
            if (newEntity == null)
            {
                throw new ArgumentNullException(nameof(newEntity));
            }

            this.dataBaseContext.messages.Add(newEntity);
            this.dataBaseContext.SaveChanges();
            return newEntity.GetId();
        }

        public void DeleteById(int identificationNumber)
        {
            var message = this.dataBaseContext.messages.FirstOrDefault(m => m.GetId() == identificationNumber);
            if (message != null)
            {
                this.dataBaseContext.messages.Remove(message);
                this.dataBaseContext.SaveChanges();
            }
        }

        public void UpdateById(int identificationNumber, Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var existingMessage = this.dataBaseContext.messages.FirstOrDefault(m => m.GetId() == identificationNumber);
            if (existingMessage != null)
            {
                this.dataBaseContext.SaveChanges();
            }
        }

        public IEnumerable<Message> GetAll()
        {
            return this.dataBaseContext.messages
                .ToList();
        }

        public Message GetById(int identificationNumber)
        {
            var message = this.dataBaseContext.messages.FirstOrDefault(m => m.GetId() == identificationNumber);
            return message ?? throw new KeyNotFoundException($"Message with id {identificationNumber} not found.");
        }

        public IEnumerable<Message> GetByChatId(int chatId)
        {
            return this.dataBaseContext.messages
                .Where(m => m.GetChat().Id == chatId)
                .OrderBy(m => m.Timestamp)
                .ToList();
        }

        public IEnumerable<Message> GetMessagesSince(int chatId, int firstMessageId)
        {
            return this.dataBaseContext.messages
                .Where(m => m.GetChat().Id == chatId && m.GetId() >= firstMessageId)
                .OrderBy(m => m.Timestamp)
                .ToList();
        }

        //public void MarkAsRead(int messageId)
        //{
        //    var message = this.dataBaseContext.messages.FirstOrDefault(m => m.GetId() == messageId);
        //    if (message != null)
        //    {
        //        message.IsRead = true;
        //        this.dataBaseContext.SaveChanges();
        //    }
        //}
    }
}