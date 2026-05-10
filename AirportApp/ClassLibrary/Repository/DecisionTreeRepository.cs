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
                .Include(n => n.Options)
                .ThenInclude(o => o.NextOption)
                .FirstOrDefaultAsync(n => n.NodeId == id);

            return node;
        }

        public async Task<int> CreateNewEntityAsync(FAQNode incomingFAQNodeEntityToBeSaved)
        {
            var nodeEntity = new FAQNode
            {
                QuestionText = incomingFAQNodeEntityToBeSaved.QuestionText,
                IsFinalAnswer = incomingFAQNodeEntityToBeSaved.IsFinalAnswer
            };

            foreach (var opt in incomingFAQNodeEntityToBeSaved.Options)
            {
                var optionEntity = new FAQOption(opt.Label, opt.NextOption);
                nodeEntity.Options.Add(optionEntity);
            }

            this.dataBaseContext.FaqNodes.Add(nodeEntity);
            await this.dataBaseContext.SaveChangesAsync();

            return nodeEntity.NodeId;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var node = await this.dataBaseContext.FaqNodes.Include(n => n.Options).FirstOrDefaultAsync(n => n.NodeId == id);
            if (node == null)
            {
                return;
            }

            this.dataBaseContext.FaqNodes.Remove(node);
            await this.dataBaseContext.SaveChangesAsync();
        }

        public async Task UpdateByIdAsync(int id, FAQNode updatedFAQNodeEntityData)
        {
            var node = await this.dataBaseContext.FaqNodes.Include(n => n.Options).FirstOrDefaultAsync(n => n.NodeId == id);
            if (node == null)
            {
                return;
            }

            node.QuestionText = updatedFAQNodeEntityData.QuestionText;
            node.IsFinalAnswer = updatedFAQNodeEntityData.IsFinalAnswer;

            node.Options.Clear();

            foreach (var opt in updatedFAQNodeEntityData.Options)
            {
                var optionEntity = new FAQOption(opt.Label, opt.NextOption);
                node.Options.Add(optionEntity);
            }

            await this.dataBaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<FAQNode>> GetAllAsync()
        {
            var nodes = await this.dataBaseContext.FaqNodes
                .Include(n => n.Options)
                .ThenInclude(o => o.NextOption)
                .ToListAsync();

            return nodes;
        }
    }
}