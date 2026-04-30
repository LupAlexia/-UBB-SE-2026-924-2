using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.Src.Model.Ticket;
using AirportApp.Src.Repository;
using AirportApp.Src.Repository.Interfaces;
using AirportApp.Src.Service.Interfaces;

public class TicketCategoryService : ITicketCategoryService
{
    private readonly ITicketCategoryRepository categoryRepository;

    public TicketCategoryService(ITicketCategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    public TicketCategory GetCategoryById(int categoryId)
    {
        return categoryRepository.GetById(categoryId);
    }
    public IEnumerable<TicketCategory> GetAllCategories()
    {
        return categoryRepository.GetAll();
    }
}