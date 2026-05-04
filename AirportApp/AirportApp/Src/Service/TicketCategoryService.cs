using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.Src.Service.Interfaces;
using AirportApp.ClassLibrary.Repository.Interfaces;

public class TicketCategoryService : ITicketCategoryService
{
    private readonly ITicketCategoryRepository categoryRepository;

    public TicketCategoryService(ITicketCategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    public async Task<TicketCategory> GetCategoryByIdAsync(int categoryId)
    {
        return await categoryRepository.GetByIdAsync(categoryId);
    }

    public async Task<IEnumerable<TicketCategory>> GetAllCategoriesAsync()
    {
        return await categoryRepository.GetAllAsync();
    }
}