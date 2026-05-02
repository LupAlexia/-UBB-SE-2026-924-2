using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.Src.Model.Ticket;
using AirportApp.Src.Repository;
using AirportApp.Src.Repository.Database;
using Microsoft.Data.SqlClient;

public class ComplaintTicketSubcategoryRepository : DatabaseRepository<int, ComplaintTicketSubcategory>, IComplaintTicketSubcategoryRepository
{
    public IEnumerable<ComplaintTicketSubcategory> GetAll()
    {
        string selectQuery = "SELECT * FROM TicketSubcategory";
        SqlCommand selectCommand = new SqlCommand(selectQuery);
        return GetAll(selectCommand);
    }

    public ComplaintTicketSubcategory GetById(int subcategoryId)
    {
        string getByIdQuery = "SELECT * FROM TicketSubcategory WHERE subcategory_id = @id";
        SqlCommand getByIdCommand = new SqlCommand(getByIdQuery);
        getByIdCommand.Parameters.AddWithValue("@id", subcategoryId);

        return GetAll(getByIdCommand).FirstOrDefault()
               ?? throw new KeyNotFoundException($"Subcategory with id {subcategoryId} not found.");
    }
    public IEnumerable<ComplaintTicketSubcategory> GetByCategoryId(int categoryId)
    {
        string selectQuery = "SELECT * FROM TicketSubcategory WHERE category_id = @categoryId";
        SqlCommand selectCommand = new SqlCommand(selectQuery);
        selectCommand.Parameters.AddWithValue("@categoryId", categoryId);

        return GetAll(selectCommand);
    }

    protected override ComplaintTicketSubcategory MapRowToEntity(SqlDataReader reader)
    {
        int subcategoryId = reader.GetInt32(reader.GetOrdinal("subcategory_id"));
        string subcategoryName = reader.GetString(reader.GetOrdinal("name"));
        int externalReferenceId = reader.GetInt32(reader.GetOrdinal("external_id"));

        int parentCategoryId = reader.GetInt32(reader.GetOrdinal("category_id"));
        var categoryRepository = new ComplaintTicketCategoryRepository();
        ComplaintTicketCategory parentCategory = categoryRepository.GetById(parentCategoryId);

        return new ComplaintTicketSubcategory(subcategoryId, subcategoryName, externalReferenceId, parentCategory);
    }

    protected override int GetEntityId(ComplaintTicketSubcategory subcategoryEntity) => subcategoryEntity.SubcategoryId;
}