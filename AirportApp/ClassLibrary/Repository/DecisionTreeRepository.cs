using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AirportApp.ClassLibrary.DataAccess;
using AirportApp.ClassLibrary.Entity.Domain.Faq.Bot;
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
                .FirstOrDefaultAsync(n => n.NodeId == id);

            if (node == null)
            {
                return null;
            }

            var options = node.Options
                .Select(o => new FAQOption(o.Label, o.NextOptionId))
                .ToImmutableArray();

            return new FAQNode(node.NodeId, node.QuestionText, options, node.IsFinalAnswer);
        }

        public async Task<int> CreateNewEntityAsync(FAQNode incomingFAQNodeEntityToBeSaved)
        {
            var nodeEntity = new FAQNodeEntity
            {
                QuestionText = incomingFAQNodeEntityToBeSaved.questionText,
                IsFinalAnswer = incomingFAQNodeEntityToBeSaved.isFinalAnswer
            };

            foreach (var opt in incomingFAQNodeEntityToBeSaved.options)
            {
                nodeEntity.Options.Add(new FAQOptionEntity { Label = opt.label, NextOptionId = opt.nextOptionId });
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

            if (node.Options != null && node.Options.Any())
            {
                this.dataBaseContext.FaqOptions.RemoveRange(node.Options);
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

            node.QuestionText = updatedFAQNodeEntityData.questionText;
            node.IsFinalAnswer = updatedFAQNodeEntityData.isFinalAnswer;

            this.dataBaseContext.FaqOptions.RemoveRange(node.Options);
            node.Options.Clear();

            foreach (var opt in updatedFAQNodeEntityData.options)
            {
                node.Options.Add(new FAQOptionEntity { NodeId = id, Label = opt.label, NextOptionId = opt.nextOptionId });
            }

            await this.dataBaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<FAQNode>> GetAllAsync()
        {
            var nodes = await this.dataBaseContext.FaqNodes
                .Include(n => n.Options)
                .ToListAsync();

            return nodes.Select(n => new FAQNode(
                n.NodeId,
                n.QuestionText,
                n.Options.Select(o => new FAQOption(o.Label, o.NextOptionId)).ToImmutableArray(),
                n.IsFinalAnswer));
        }
    }
}