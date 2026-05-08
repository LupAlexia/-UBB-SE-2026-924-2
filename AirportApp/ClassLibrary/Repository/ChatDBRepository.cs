using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<int> CreateNewEntityAsync(Chat incomingChatEntityToBeSaved)
        {
            if (incomingChatEntityToBeSaved == null)
            {
                throw new ArgumentNullException(nameof(incomingChatEntityToBeSaved));
            }

            this.dataBaseContext.Chats.Add(incomingChatEntityToBeSaved);
            await this.dataBaseContext.SaveChangesAsync();
            return incomingChatEntityToBeSaved.Id;
        }

        public async Task DeleteByIdAsync(int identifierForChatToBeDeleted)
        {
            var chat = await this.dataBaseContext.Chats.FirstOrDefaultAsync(c => c.Id == identifierForChatToBeDeleted);
            if (chat != null)
            {
                this.dataBaseContext.Chats.Remove(chat);
                await this.dataBaseContext.SaveChangesAsync();
            }
        }

        public async Task UpdateByIdAsync(int identifierForChatToBeUpdated, Chat updatedChatEntityData)
        {
            if (updatedChatEntityData == null)
            {
                throw new ArgumentNullException(nameof(updatedChatEntityData));
            }

            var chatFound = await this.dataBaseContext.Chats.FirstOrDefaultAsync(c => c.Id == identifierForChatToBeUpdated);
            if (chatFound != null)
            {
                chatFound.Status = updatedChatEntityData.Status;
                await this.dataBaseContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Chat>> GetAllAsync()
        {
            return await this.dataBaseContext.Chats.ToListAsync();
        }

        public async Task<Chat> GetByIdAsync(int identifierForRequestedChat)
        {
            var chat = await this.dataBaseContext.Chats.FirstOrDefaultAsync(c => c.Id == identifierForRequestedChat);
            return chat ?? throw new KeyNotFoundException($"Chat with id {identifierForRequestedChat} not found.");
        }
    }
}