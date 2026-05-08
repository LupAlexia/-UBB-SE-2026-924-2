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
    public class MessageDatabaseRepository : IMessageRepository
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

            this.dataBaseContext.Messages.Add(newEntity);
            await this.dataBaseContext.SaveChangesAsync();
            return newEntity.Id;
        }

        public async Task DeleteByIdAsync(int identificationNumber)
        {
            var message = await this.dataBaseContext.Messages.FirstOrDefaultAsync(m => m.Id == identificationNumber);
            if (message != null)
            {
                this.dataBaseContext.Messages.Remove(message);
                await this.dataBaseContext.SaveChangesAsync();
            }
        }

        public async Task UpdateByIdAsync(int identificationNumber, Message message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            var existingMessage = await this.dataBaseContext.Messages.FirstOrDefaultAsync(m => m.Id == identificationNumber);
            if (existingMessage != null)
            {
                await this.dataBaseContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Message>> GetAllAsync()
        {
            return await this.dataBaseContext.Messages.ToListAsync();
        }

        public async Task<Message> GetByIdAsync(int identificationNumber)
        {
            var message = await this.dataBaseContext.Messages.FirstOrDefaultAsync(m => m.Id == identificationNumber);
            return message ?? throw new KeyNotFoundException($"Message with id {identificationNumber} not found.");
        }

        public async Task<IEnumerable<Message>> GetByChatIdAsync(int chatId)
        {
            return await this.dataBaseContext.Messages
                .Where(m => m.Chat.Id == chatId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int firstMessageId)
        {
            return await this.dataBaseContext.Messages
                .Where(m => m.Chat.Id == chatId && m.Id >= firstMessageId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }
    }
}
