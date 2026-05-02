using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.Src.Model.Ticket;
using AirportApp.Src.Repository;
using AirportApp.Src.Repository.Interfaces;
using AirportApp.Src.Service.Interfaces;

public class ComplaintTicketSubcategoryService : IComplaintTicketSubcategoryService
{
    private readonly IComplaintTicketSubcategoryRepository subcategoryRepository;

    public ComplaintTicketSubcategoryService(IComplaintTicketSubcategoryRepository subcategoryRepository)
    {
        this.subcategoryRepository = subcategoryRepository;
    }

    public IEnumerable<ComplaintTicketSubcategory> GetSubcategoriesByCategoryId(int categoryId)
    {
        return subcategoryRepository.GetByCategoryId(categoryId);
    }
    public ComplaintTicketSubcategory GetSubcategoryById(int subcategoryId)
    {
        return subcategoryRepository.GetById(subcategoryId);
    }
    // public IEnumerable<TicketCategory> GetAllSubcategories()
    // {
    //    return _subcategoryRepository.GetAll();
    // }
}