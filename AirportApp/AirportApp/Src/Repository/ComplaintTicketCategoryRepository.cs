using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.Src.Model.Ticket;
using AirportApp.Src.Repository.Interfaces;
using AirportApp.Src.Repository.Database;
using AirportApp.Src.Repository.Database;
using Microsoft.Data.SqlClient;

using AirportApp.Src.Repository;

public class ComplaintTicketCategoryRepository : DatabaseRepository<int, ComplaintTicketCategory>, IComplaintTicketCategoryRepository
{
    public IEnumerable<ComplaintTicketCategory> GetAll()
    {
        string selectQuery = "SELECT * FROM TicketCategory";
        SqlCommand selectCommand = new SqlCommand(selectQuery);
        return GetAll(selectCommand);
    }

    public ComplaintTicketCategory GetById(int categoryId)
    {
        string selectQuery = "SELECT * FROM TicketCategory WHERE category_id = @id";
        SqlCommand selectCommand = new SqlCommand(selectQuery);
        selectCommand.Parameters.AddWithValue("@id", categoryId);
        return GetById(categoryId, selectCommand);
    }

    protected override ComplaintTicketCategory MapRowToEntity(SqlDataReader reader)
    {
        int categoryId = reader.GetInt32(reader.GetOrdinal("category_id"));
        string categoryName = reader.GetString(reader.GetOrdinal("name"));
        string urgencyLevelString = reader.GetString(reader.GetOrdinal("urgency_level"));

        if (!Enum.TryParse<ComplaintTicketUrgencyLevelEnum>(urgencyLevelString, true, out var urgencyLevel))
        {
            urgencyLevel = ComplaintTicketUrgencyLevelEnum.LOW;
        }
        return new ComplaintTicketCategory(categoryId, categoryName, urgencyLevel);
    }

    protected override int GetEntityId(ComplaintTicketCategory categoryEntity) => categoryEntity.CategoryId;
}