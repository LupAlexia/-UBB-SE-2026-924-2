using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.Src.Model.Ticket;
using AirportApp.Src.Repository;
using AirportApp.Src.Repository.Interfaces;
using AirportApp.Src.Service.Interfaces;

public class ComplaintTicketCategoryService : IComplaintTicketCategoryService
{
    private readonly IComplaintTicketCategoryRepository categoryRepository;

    public ComplaintTicketCategoryService(IComplaintTicketCategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    public ComplaintTicketCategory GetCategoryById(int categoryId)
    {
        return categoryRepository.GetById(categoryId);
    }
    public IEnumerable<ComplaintTicketCategory> GetAllCategories()
    {
        return categoryRepository.GetAll();
    }
}