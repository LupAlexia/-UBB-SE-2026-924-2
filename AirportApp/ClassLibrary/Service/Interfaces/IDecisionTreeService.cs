using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;

namespace AirportApp.ClassLibrary.Service.Interfaces
{
    public interface IDecisionTreeService
    {
        Task<IEnumerable<FAQNode>> GetAllNodesAsync();
        Task<FAQNode> GetNodeByIdAsync(int id);
        Task<int> CreateNodeAsync(FAQNode node);
        Task UpdateNodeAsync(int id, FAQNode node);
        Task DeleteNodeAsync(int id);
    }
}