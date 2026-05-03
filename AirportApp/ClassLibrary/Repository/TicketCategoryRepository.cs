using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using Microsoft.Data.SqlClient;
using AirportApp.ClassLibrary.Repository.Interfaces;
namespace AirportApp.ClassLibrary.Repository
{
    public class TicketCategoryRepository : DatabaseRepository<int, TicketCategory>, ITicketCategoryRepository
    {
        public IEnumerable<TicketCategory> GetAll()
        {
            string selectQuery = "SELECT * FROM TicketCategory";
            SqlCommand selectCommand = new SqlCommand(selectQuery);
            return GetAll(selectCommand);
        }

        public TicketCategory GetById(int categoryId)
        {
            string selectQuery = "SELECT * FROM TicketCategory WHERE category_id = @id";
            SqlCommand selectCommand = new SqlCommand(selectQuery);
            selectCommand.Parameters.AddWithValue("@id", categoryId);
            return GetById(categoryId, selectCommand);
        }

        protected override TicketCategory MapRowToEntity(SqlDataReader reader)
        {
            int categoryId = reader.GetInt32(reader.GetOrdinal("category_id"));
            string categoryName = reader.GetString(reader.GetOrdinal("name"));
            string urgencyLevelString = reader.GetString(reader.GetOrdinal("urgency_level"));

            if (!Enum.TryParse<TicketUrgencyLevelEnum>(urgencyLevelString, true, out var urgencyLevel))
            {
                urgencyLevel = TicketUrgencyLevelEnum.LOW;
            }
            return new TicketCategory(categoryId, categoryName, urgencyLevel);
        }

        protected override int GetEntityId(TicketCategory categoryEntity) => categoryEntity.CategoryId;
    }
}