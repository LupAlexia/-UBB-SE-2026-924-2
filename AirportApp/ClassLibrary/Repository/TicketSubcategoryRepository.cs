using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain.Ticket;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.Data.SqlClient;

namespace AirportApp.ClassLibrary.Repository
{
    public class TicketSubcategoryRepository : DatabaseRepository<int, TicketSubcategory>, ITicketSubcategoryRepository
    {
        public IEnumerable<TicketSubcategory> GetAll()
        {
            string selectQuery = "SELECT * FROM TicketSubcategory";
            SqlCommand selectCommand = new SqlCommand(selectQuery);
            return GetAll(selectCommand);
        }

        public TicketSubcategory GetById(int subcategoryId)
        {
            string getByIdQuery = "SELECT * FROM TicketSubcategory WHERE subcategory_id = @id";
            SqlCommand getByIdCommand = new SqlCommand(getByIdQuery);
            getByIdCommand.Parameters.AddWithValue("@id", subcategoryId);

            return GetAll(getByIdCommand).FirstOrDefault()
                   ?? throw new KeyNotFoundException($"Subcategory with id {subcategoryId} not found.");
        }
        public IEnumerable<TicketSubcategory> GetByCategoryId(int categoryId)
        {
            string selectQuery = "SELECT * FROM TicketSubcategory WHERE category_id = @categoryId";
            SqlCommand selectCommand = new SqlCommand(selectQuery);
            selectCommand.Parameters.AddWithValue("@categoryId", categoryId);

            return GetAll(selectCommand);
        }

        protected override TicketSubcategory MapRowToEntity(SqlDataReader reader)
        {
            int subcategoryId = reader.GetInt32(reader.GetOrdinal("subcategory_id"));
            string subcategoryName = reader.GetString(reader.GetOrdinal("name"));
            int externalReferenceId = reader.GetInt32(reader.GetOrdinal("external_id"));

            int parentCategoryId = reader.GetInt32(reader.GetOrdinal("category_id"));
            var categoryRepository = new TicketCategoryRepository();
            TicketCategory parentCategory = categoryRepository.GetById(parentCategoryId);

            return new TicketSubcategory(subcategoryId, subcategoryName, externalReferenceId, parentCategory);
        }

        protected override int GetEntityId(TicketSubcategory subcategoryEntity) => subcategoryEntity.SubcategoryId;
    }
}