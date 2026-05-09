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
            var nodes = await this.dataBaseContext.FaqNodes
                .Include(n => n.Options)
                .ToListAsync();

            var node = nodes.FirstOrDefault(n => n.NodeId == id);

            if (node == null)
            {
                return null;
            }

            var nodesById = nodes.ToDictionary(n => n.NodeId);
            var mappedNode = MapNode(node, nodesById, new Dictionary<int, FAQNode>());

            return mappedNode;
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
                this.dataBaseContext.Entry(optionEntity).Property<int?>("NextOptionId").CurrentValue = opt.NextOption?.NodeId;
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
                this.dataBaseContext.Entry(optionEntity).Property<int?>("NextOptionId").CurrentValue = opt.NextOption?.NodeId;
            }

            await this.dataBaseContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<FAQNode>> GetAllAsync()
        {
            var nodes = await this.dataBaseContext.FaqNodes
                .Include(n => n.Options)
                .ToListAsync();

            var nodesById = nodes.ToDictionary(n => n.NodeId);
            var cache = new Dictionary<int, FAQNode>();

            return nodes.Select(n => MapNode(n, nodesById, cache));
        }

        private FAQNode MapNode(
            FAQNode source,
            IReadOnlyDictionary<int, FAQNode> nodesById,
            IDictionary<int, FAQNode> cache)
        {
            if (cache.TryGetValue(source.NodeId, out var mappedNode))
            {
                return mappedNode;
            }

            mappedNode = new FAQNode
            {
                NodeId = source.NodeId,
                QuestionText = source.QuestionText,
                IsFinalAnswer = source.IsFinalAnswer
            };

            cache[source.NodeId] = mappedNode;

            mappedNode.Options = source.Options
                .Select(option => new FAQOption(option.Label, ResolveNextNodeId(option))
                {
                    NextOption = ResolveNextNode(option, nodesById, cache)
                })
                .ToList();

            return mappedNode;
        }

        private int ResolveNextNodeId(FAQOption option)
        {
            var trackedEntry = this.dataBaseContext.Entry(option);
            var nextOptionId = trackedEntry.Property<int?>("NextOptionId").CurrentValue;

            if (nextOptionId.HasValue)
            {
                return nextOptionId.Value;
            }

            return option.NextOption?.NodeId ?? 0;
        }

        private FAQNode? ResolveNextNode(
            FAQOption option,
            IReadOnlyDictionary<int, FAQNode> nodesById,
            IDictionary<int, FAQNode> cache)
        {
            var nextOptionId = ResolveNextNodeId(option);

            return nextOptionId != 0 && nodesById.TryGetValue(nextOptionId, out var nextNode)
                ? MapNode(nextNode, nodesById, cache)
                : null;
        }
    }
}