using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.ClassLibrary.Entity.Repository.Database
{
    public class DecisionTreeRepository : IRepository<int, FAQNode>
    {
        private readonly AirportDbContext dataBaseContext;

        public DecisionTreeRepository(AirportDbContext dataBaseContext)
        {
            this.dataBaseContext = dataBaseContext ?? throw new ArgumentNullException(nameof(dataBaseContext));
        }

        public async Task<FAQNode> GetByIdAsync(int id)
        {
            var node = await this.dataBaseContext.FaqNodes
                .Include(nodeEntity => nodeEntity.Options)
                .ThenInclude(option => option.NextOption)
                .FirstOrDefaultAsync(nodeEntity => nodeEntity.NodeId == id);

            return node ?? throw new KeyNotFoundException($"FAQNode with id {id} was not found.");
        }

        public async Task<int> CreateNewEntityAsync(FAQNode incomingFAQNodeEntityToBeSaved)
        {
            var nodeEntity = new FAQNode
            {
                QuestionText = incomingFAQNodeEntityToBeSaved.QuestionText,
                IsFinalAnswer = incomingFAQNodeEntityToBeSaved.IsFinalAnswer
            };

            foreach (var option in incomingFAQNodeEntityToBeSaved.Options)
            {
                var optionEntity = new FAQOption(option.Label, option.NextOption);
                nodeEntity.Options.Add(optionEntity);
            }

            this.dataBaseContext.FaqNodes.Add(nodeEntity);
            await this.dataBaseContext.SaveChangesAsync();

            return nodeEntity.NodeId;
        }

        public async Task DeleteByIdAsync(int nodeIdentifier)
        {
            var node = await this.dataBaseContext.FaqNodes.Include(nodeEntity => nodeEntity.Options).FirstOrDefaultAsync(nodeEntity => nodeEntity.NodeId == nodeIdentifier);
            if (node == null)
            {
                return;
            }

            this.dataBaseContext.FaqNodes.Remove(node);
            await this.dataBaseContext.SaveChangesAsync();
        }

        public async Task UpdateByIdAsync(int id, FAQNode updatedFAQNodeEntityData)
        {
            var node = await this.dataBaseContext.FaqNodes.Include(node => node.Options).FirstOrDefaultAsync(node => node.NodeId == id);
            if (node == null)
            {
                return;
            }

            node.QuestionText = updatedFAQNodeEntityData.QuestionText;
            node.IsFinalAnswer = updatedFAQNodeEntityData.IsFinalAnswer;

            node.Options.Clear();

            foreach (var option in updatedFAQNodeEntityData.Options)
            {
                var optionEntity = new FAQOption(option.Label, option.NextOption);
                node.Options.Add(optionEntity);
            }

            await this.dataBaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<FAQNode>> GetAllAsync()
        {
            var nodes = await this.dataBaseContext.FaqNodes
                .Include(node => node.Options)
                .ThenInclude(option => option.NextOption)
                .ToListAsync();

            return nodes;
        }
    }
}