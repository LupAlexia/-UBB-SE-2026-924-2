using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
            this.dataBaseContext = dataBaseContext ?? throw new ArgumentNullException(nameof(dataBaseContext)); ;
        }

        public FAQNode GetById(int id)
        {
            var node = this.dataBaseContext.faqNodes
                .Include(n => n.Options)
                .FirstOrDefault(n => n.NodeId == id);

            if (node == null) return null;

            var options = node.Options
                .Select(o => new FAQOption(o.Label, o.NextOptionId))
                .ToImmutableArray();

            return new FAQNode(node.NodeId, node.QuestionText, options, node.IsFinalAnswer);
        }

        public int CreateNewEntity(FAQNode incomingFAQNodeEntityToBeSaved)
        {
            var nodeEntity = new FAQNodeEntity
            {
                QuestionText = incomingFAQNodeEntityToBeSaved.QuestionText,
                IsFinalAnswer = incomingFAQNodeEntityToBeSaved.IsFinalAnswer
            };

            foreach (var opt in incomingFAQNodeEntityToBeSaved.Options)
            {
                nodeEntity.Options.Add(new FAQOptionEntity { Label = opt.Label, NextOptionId = opt.NextOptionId });
            }

            this.dataBaseContext.faqNodes.Add(nodeEntity);
            this.dataBaseContext.SaveChanges();

            return nodeEntity.NodeId;
        }

        public void DeleteById(int id)
        {
            var node = this.dataBaseContext.faqNodes.Include(n => n.Options).FirstOrDefault(n => n.NodeId == id);
            if (node == null) return;

            // Remove options first
            if (node.Options != null && node.Options.Any())
            {
                this.dataBaseContext.faqOptions.RemoveRange(node.Options);
            }

            this.dataBaseContext.faqNodes.Remove(node);
            this.dataBaseContext.SaveChanges();
        }

        public void UpdateById(int id, FAQNode updatedFAQNodeEntityData)
        {
            var node = this.dataBaseContext.faqNodes.Include(n => n.Options).FirstOrDefault(n => n.NodeId == id);
            if (node == null) return;

            node.QuestionText = updatedFAQNodeEntityData.QuestionText;
            node.IsFinalAnswer = updatedFAQNodeEntityData.IsFinalAnswer;

            // Replace options
            this.dataBaseContext.faqOptions.RemoveRange(node.Options);
            node.Options.Clear();

            foreach (var opt in updatedFAQNodeEntityData.Options)
            {
                node.Options.Add(new FAQOptionEntity { NodeId = id, Label = opt.Label, NextOptionId = opt.NextOptionId });
            }

            this.dataBaseContext.SaveChanges();
        }

        public IEnumerable<FAQNode> GetAll()
        {
            var nodes = this.dataBaseContext.faqNodes
                .Include(n => n.Options)
                .ToList();

            return nodes.Select(n => new FAQNode(
                n.NodeId,
                n.QuestionText,
                n.Options.Select(o => new FAQOption(o.Label, o.NextOptionId)).ToImmutableArray(),
                n.IsFinalAnswer));
        }
    }
}