using System.Collections.Generic;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Service.Interfaces;
using AirportApp.ClassLibrary.Repository.Interfaces;
using AirportApp.ClassLibrary.Entity.Domain;

public class ComplaintTicketCategoryService : IComplaintTicketCategoryService
{
    private readonly ITicketCategoryRepository categoryRepository;

    public ComplaintTicketCategoryService(ITicketCategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    public async Task<ComplaintTicketCategory> GetCategoryByIdAsync(int categoryId)
    {
        return await categoryRepository.GetByIdAsync(categoryId);
    }

    public async Task<IEnumerable<ComplaintTicketCategory>> GetAllCategoriesAsync()
    {
        return await categoryRepository.GetAllAsync();
    }
}