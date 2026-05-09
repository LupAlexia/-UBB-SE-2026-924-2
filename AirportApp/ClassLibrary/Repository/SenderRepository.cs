using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.ClassLibrary.Repository
{
    public class SenderRepository : IRepository<int, Sender>
    {
        private readonly AirportDbContext dataBaseContext;

        public SenderRepository(AirportDbContext context)
        {
            dataBaseContext = context ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public async Task<Sender> GetByIdAsync(int id)
        {
            // Special-case the bot engine identity (sentinel id)
            if (id == BotEngineIdentity.CONSTANT_IDENTIFIER_FOR_DEFAULT_BOT_SYSTEM_USER)
            {
                // Return the bot sentinel object, not a persisted user row.
                return new BotEngineIdentity(null);
            }

            return await dataBaseContext.Senders
                .FirstOrDefaultAsync(s => s.Id == id)
                ?? throw new KeyNotFoundException($"Sender with id {id} was not found.");
        }

        public async Task<IEnumerable<Sender>> GetAllAsync()
        {
            return await dataBaseContext.Senders.ToListAsync();
        }

        public async Task<int> CreateNewEntityAsync(Sender senderElement)
        {
            if (senderElement == null)
            {
                throw new ArgumentNullException(nameof(senderElement));
            }

            dataBaseContext.Senders.Add(senderElement);
            await dataBaseContext.SaveChangesAsync();
            return senderElement.Id;
        }

        public async Task UpdateByIdAsync(int id, Sender senderElement)
        {
            if (senderElement == null)
            {
                throw new ArgumentNullException(nameof(senderElement));
            }

            dataBaseContext.Senders.Update(senderElement);
            await dataBaseContext.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var sender = await dataBaseContext.Senders.FindAsync(id);
            if (sender != null)
            {
                dataBaseContext.Senders.Remove(sender);
                await dataBaseContext.SaveChangesAsync();
            }
        }
    }
}
