using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.Src.Service.Interfaces;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Service
{
    public class TicketSubcategoryService : ITicketSubcategoryService
    {
        private readonly ITicketSubcategoryRepository subcategoryRepository;

        public TicketSubcategoryService(ITicketSubcategoryRepository subcategoryRepository)
        {
            this.subcategoryRepository = subcategoryRepository;
        }

        public async Task<IEnumerable<TicketSubcategory>> GetSubcategoriesByCategoryIdAsync(int categoryId)
        {
            return await subcategoryRepository.GetByCategoryIdAsync(categoryId);
        }

        public async Task<TicketSubcategory> GetSubcategoryByIdAsync(int subcategoryId)
        {
            return await subcategoryRepository.GetByIdAsync(subcategoryId);
        }
    }
}