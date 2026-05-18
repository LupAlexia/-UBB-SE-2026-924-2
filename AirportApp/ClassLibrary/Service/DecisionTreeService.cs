using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Service.Interfaces;

namespace AirportApp.Src.Service
{
    public class DecisionTreeService : IDecisionTreeService
    {
        private readonly IRepository<int, FAQNode> decisionTreeRepository;

        public DecisionTreeService(IRepository<int, FAQNode> decisionTreeRepository)
        {
            this.decisionTreeRepository = decisionTreeRepository;
        }

        public async Task<IEnumerable<FAQNode>> GetAllNodesAsync()
        {
            return await decisionTreeRepository.GetAllAsync();
        }

        public async Task<FAQNode> GetNodeByIdAsync(int id)
        {
            return await decisionTreeRepository.GetByIdAsync(id);
        }

        public async Task<int> CreateNodeAsync(FAQNode node)
        {
            return await decisionTreeRepository.CreateNewEntityAsync(node);
        }

        public async Task UpdateNodeAsync(int id, FAQNode node)
        {
            await decisionTreeRepository.UpdateByIdAsync(id, node);
        }

        public async Task DeleteNodeAsync(int id)
        {
            await decisionTreeRepository.DeleteByIdAsync(id);
        }
    }
}