using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.Src.Service.Interfaces;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.Src.Service
{
    public class ComplaintTicketSubcategoryService : IComplaintTicketSubcategoryService
    {
        private readonly ITicketSubcategoryRepository subcategoryRepository;

        public ComplaintTicketSubcategoryService(ITicketSubcategoryRepository subcategoryRepository)
        {
            this.subcategoryRepository = subcategoryRepository;
        }

        public async Task<IEnumerable<ComplaintTicketSubcategory>> GetSubcategoriesByCategoryIdAsync(int categoryId)
        {
            return await subcategoryRepository.GetByCategoryIdAsync(categoryId);
        }

        public async Task<ComplaintTicketSubcategory> GetSubcategoryByIdAsync(int subcategoryId)
        {
            return await subcategoryRepository.GetByIdAsync(subcategoryId);
        }
    }
}