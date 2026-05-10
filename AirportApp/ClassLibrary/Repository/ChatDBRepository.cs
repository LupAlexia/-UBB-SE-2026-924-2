using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Repository
{
    public class ChatDatabaseRepository : IRepository<int, Chat>
    {
        private readonly AirportDbContext databaseContext;

        public ChatDatabaseRepository(AirportDbContext databaseContext)
        {
            this.databaseContext = databaseContext ?? throw new ArgumentNullException(nameof(databaseContext));
        }

        public async Task<int> CreateNewEntityAsync(Chat incomingChatEntityToBeSaved)
        {
            if (incomingChatEntityToBeSaved == null)
            {
                throw new ArgumentNullException(nameof(incomingChatEntityToBeSaved));
            }

            if (incomingChatEntityToBeSaved.User == null || incomingChatEntityToBeSaved.User.Id <= 0)
            {
                throw new ArgumentException("Chat must contain a valid user id.", nameof(incomingChatEntityToBeSaved));
            }

            var chatToPersist = new Chat
            {
                Status = incomingChatEntityToBeSaved.Status
            };

            this.databaseContext.Chats.Add(chatToPersist);
            this.databaseContext.Entry(chatToPersist).Property("UserId").CurrentValue = incomingChatEntityToBeSaved.User.Id;
            await this.databaseContext.SaveChangesAsync();
            return chatToPersist.Id;
        }

        public async Task DeleteByIdAsync(int identifierForChatToBeDeleted)
        {
            var chat = await this.databaseContext.Chats.FirstOrDefaultAsync(chat => chat.Id == identifierForChatToBeDeleted);
            if (chat != null)
            {
                this.databaseContext.Chats.Remove(chat);
                await this.databaseContext.SaveChangesAsync();
            }
        }

        public async Task UpdateByIdAsync(int identifierForChatToBeUpdated, Chat updatedChatEntityData)
        {
            if (updatedChatEntityData == null)
            {
                throw new ArgumentNullException(nameof(updatedChatEntityData));
            }

            var chatFound = await this.databaseContext.Chats.FirstOrDefaultAsync(chatEntity => chatEntity.Id == identifierForChatToBeUpdated);
            if (chatFound != null)
            {
                chatFound.Status = updatedChatEntityData.Status;
                await this.databaseContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Chat>> GetAllAsync()
        {
            return await this.databaseContext.Chats.ToListAsync();
        }

        public async Task<Chat> GetByIdAsync(int identifierForRequestedChat)
        {
            var chat = await this.databaseContext.Chats
                .Include(chatEntity => chatEntity.User)
                .FirstOrDefaultAsync(chatEntity => chatEntity.Id == identifierForRequestedChat);

            return chat ?? throw new KeyNotFoundException($"Chat with id {identifierForRequestedChat} not found.");
        }
    }
}