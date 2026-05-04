using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<int> CreateNewEntityAsync(Message newEntity)
        {
            if (newEntity == null)
            {
                throw new ArgumentNullException(nameof(newEntity));
            }

            this.dataBaseContext.messages.Add(newEntity);
            await this.dataBaseContext.SaveChangesAsync();
            return newEntity.GetId();
        }

        public async Task DeleteByIdAsync(int identificationNumber)
        {
            var message = await this.dataBaseContext.messages.FirstOrDefaultAsync(m => m.GetId() == identificationNumber);
            if (message != null)
            {
                this.dataBaseContext.messages.Remove(message);
                await this.dataBaseContext.SaveChangesAsync();
            }
        }

        public async Task UpdateByIdAsync(int identificationNumber, Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var existingMessage = await this.dataBaseContext.messages.FirstOrDefaultAsync(m => m.GetId() == identificationNumber);
            if (existingMessage != null)
            {
                await this.dataBaseContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Message>> GetAllAsync()
        {
            return await this.dataBaseContext.messages.ToListAsync();
        }

        public async Task<Message> GetByIdAsync(int identificationNumber)
        {
            var message = await this.dataBaseContext.messages.FirstOrDefaultAsync(m => m.GetId() == identificationNumber);
            return message ?? throw new KeyNotFoundException($"Message with id {identificationNumber} not found.");
        }

        public async Task<IEnumerable<Message>> GetByChatIdAsync(int chatId)
        {
            return await this.dataBaseContext.messages
                .Where(m => m.GetChat().Id == chatId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int firstMessageId)
        {
            return await this.dataBaseContext.messages
                .Where(m => m.GetChat().Id == chatId && m.GetId() >= firstMessageId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }
    }
}