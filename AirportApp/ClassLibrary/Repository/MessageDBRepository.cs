using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.Entity.Domain.Message;
using AirportApp.ClassLibrary.Entity.Domain;
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

            var chatId = newEntity.Chat?.Id ?? throw new InvalidOperationException("Message chat is required.");
            var senderId = newEntity.Sender?.RetrieveUniqueDatabaseIdentifierForBot() ?? throw new InvalidOperationException("Message sender is required.");

            this.dataBaseContext.Entry(newEntity).Property("ChatId").CurrentValue = chatId;
            this.dataBaseContext.Entry(newEntity).Property("SenderId").CurrentValue = senderId;
            newEntity.Chat = null!;
            newEntity.Sender = null!;

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
            var messages = await this.dataBaseContext.Messages
                .Include(m => m.Chat)
                .ToListAsync();

            await PopulateSendersAsync(messages);
            return messages;
        }

        public async Task<Message> GetByIdAsync(int identificationNumber)
        {
            var message = await this.dataBaseContext.Messages
                .Include(m => m.Chat)
                .FirstOrDefaultAsync(m => m.Id == identificationNumber);

            if (message != null)
            {
                message.Sender = await ResolveSenderAsync(message);
            }

            return message ?? throw new KeyNotFoundException($"Message with id {identificationNumber} not found.");
        }

        public async Task<IEnumerable<Message>> GetByChatIdAsync(int chatId)
        {
            var messages = await this.dataBaseContext.Messages
                .Include(m => m.Chat)
                .Where(m => m.Chat.Id == chatId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            await PopulateSendersAsync(messages);
            return messages;
        }

        public async Task<IEnumerable<Message>> GetMessagesSinceAsync(int chatId, int firstMessageId)
        {
            var messages = await this.dataBaseContext.Messages
                .Include(m => m.Chat)
                .Where(m => m.Chat.Id == chatId && m.Id >= firstMessageId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            await PopulateSendersAsync(messages);
            return messages;
        }

        private async Task PopulateSendersAsync(IEnumerable<Message> messages)
        {
            foreach (var message in messages)
            {
                message.Sender = await ResolveSenderAsync(message);
            }
        }

        private async Task<Sender> ResolveSenderAsync(Message message)
        {
            var senderId = dataBaseContext.Entry(message).Property<int>("SenderId").CurrentValue;

            if (senderId == AirportApp.ClassLibrary.Entity.Domain.Message.BotEngineIdentity.CONSTANT_IDENTIFIER_FOR_DEFAULT_BOT_SYSTEM_USER)
            {
                return new AirportApp.ClassLibrary.Entity.Domain.Message.BotEngineIdentity(null);
            }

            return await dataBaseContext.Senders
                .FirstOrDefaultAsync(s => s.Id == senderId)
                ?? throw new KeyNotFoundException($"Sender with id {senderId} was not found.");
        }
    }
}
