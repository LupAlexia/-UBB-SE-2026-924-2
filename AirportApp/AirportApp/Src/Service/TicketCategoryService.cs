using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public TicketCategory GetCategoryById(int categoryId)
    {
        return categoryRepository.GetById(categoryId);
    }
    public IEnumerable<TicketCategory> GetAllCategories()
    {
        return categoryRepository.GetAll();
    }
}