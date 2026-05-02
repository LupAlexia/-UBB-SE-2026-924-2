using System.Collections.Generic;
using System;
using AirportApp.Src.Model.Ticket;
using Microsoft.Data.SqlClient;
using AirportApp.Src.Model;
using AirportApp.Src.Model.Faq;
using AirportApp.Src.Repository.Interfaces;
using AirportApp.Src.Repository.Database;

namespace AirportApp.Src.Repository
{
    public class ComplaintTicketRepository : DatabaseRepository<int, ComplaintTicket>, IComplaintTicketRepository
    {
        private IUserRepository userRepository = new UserRepository();

        private IComplaintTicketCategoryRepository categoryRepository = new ComplaintTicketCategoryRepository();
        private IComplaintTicketSubcategoryRepository subcategoryRepository = new ComplaintTicketSubcategoryRepository();
        public ComplaintTicketRepository()
        {
        }

        public ComplaintTicket GetById(int identificationNumber)
        {
            string selectQuery = "SELECT * FROM Ticket WHERE ticket_id = @TicketId";
            SqlCommand selectCommand = new SqlCommand(selectQuery);
            selectCommand.Parameters.AddWithValue("@TicketId", identificationNumber);

            ComplaintTicket ticket = GetById(identificationNumber, selectCommand);

            if (ticket == null)
            {
                throw new KeyNotFoundException($"Ticket with TicketId {identificationNumber} was not found.");
            }

            return ticket;
        }

        public IEnumerable<ComplaintTicket> GetAll()
        {
            string selectQuery = "SELECT * FROM Ticket";
            SqlCommand selectCommand = new SqlCommand(selectQuery);
            return GetAll(selectCommand);
        }

        public int CreateNewEntity(ComplaintTicket ticketEntity)
        {
            if (ticketEntity == null)
            {
                throw new ArgumentNullException(nameof(ticketEntity), "Ticket can't be null.");
            }

            string createNewQuery = @"INSERT INTO Ticket 
                (user_id, status, category_id, subcategory_id, subject, description, created_at, urgency_level) " +
                "OUTPUT INSERTED.Ticket_id " +
                "VALUES (@userId, @status, @categoryId, @subcategoryId, @subject, @description, @creationTimestamp, @urgency)";

            SqlCommand createNewCommand = new SqlCommand(createNewQuery);

            createNewCommand.Parameters.AddWithValue("@userId", ticketEntity.Creator.UserId);
            createNewCommand.Parameters.AddWithValue("@status", ticketEntity.CurrentStatus.ToString());
            createNewCommand.Parameters.AddWithValue("@categoryId", ticketEntity.Category.CategoryId);
            createNewCommand.Parameters.AddWithValue("@subcategoryId", ticketEntity.Subcategory.SubcategoryId);
            createNewCommand.Parameters.AddWithValue("@subject", ticketEntity.Subject);
            createNewCommand.Parameters.AddWithValue("@description", ticketEntity.Description);
            createNewCommand.Parameters.AddWithValue("@creationTimestamp", ticketEntity.CreationTimestamp);
            createNewCommand.Parameters.AddWithValue("@urgency", ticketEntity.UrgencyLevel.ToString());

            int identificationNumber = Add(createNewCommand, ticketEntity);
            return identificationNumber;
        }

        public void UpdateById(int ticketId, ComplaintTicket ticketEntity)
        {
            if (ticketEntity == null)
            {
                throw new ArgumentNullException(nameof(ticketEntity), "Ticket can't be null.");
            }

            string updateTicketQuery = @"UPDATE Ticket SET 
                user_id = @userId, 
                status = @status, 
                category_id = @categoryId, 
                subcategory_id = @subcategoryId, 
                subject = @subject, 
                description = @description, 
                created_at = @creationTimestamp, 
                urgency_level = @urgency 
                WHERE ticket_id = @TicketId";

            SqlCommand updateTicketCommand = new SqlCommand(updateTicketQuery);
            updateTicketCommand.Parameters.AddWithValue("@TicketId", ticketId);
            updateTicketCommand.Parameters.AddWithValue("@userId", ticketEntity.Creator.UserId);
            updateTicketCommand.Parameters.AddWithValue("@status", ticketEntity.CurrentStatus.ToString());
            updateTicketCommand.Parameters.AddWithValue("@categoryId", ticketEntity.Category.CategoryId);
            updateTicketCommand.Parameters.AddWithValue("@subcategoryId", ticketEntity.Subcategory.SubcategoryId);
            updateTicketCommand.Parameters.AddWithValue("@subject", ticketEntity.Subject);
            updateTicketCommand.Parameters.AddWithValue("@description", ticketEntity.Description);
            updateTicketCommand.Parameters.AddWithValue("@creationTimestamp", ticketEntity.CreationTimestamp);
            updateTicketCommand.Parameters.AddWithValue("@urgency", ticketEntity.UrgencyLevel.ToString());

            UpdateById(ticketId, updateTicketCommand, ticketEntity);
        }

        public void DeleteById(int ticketId)
        {
            string deleteQuery = "DELETE FROM Ticket WHERE ticket_id = @TicketId";
            SqlCommand deleteCommand = new SqlCommand(deleteQuery);
            deleteCommand.Parameters.AddWithValue("@TicketId", ticketId);

            DeleteById(ticketId, deleteCommand);
        }

        protected override ComplaintTicket MapRowToEntity(SqlDataReader reader)
        {
            int ticketId = reader.GetInt32(reader.GetOrdinal("ticket_id"));
            int userId = reader.GetInt32(reader.GetOrdinal("user_id"));
            ComplaintTicketStatusEnum status = Enum.Parse<ComplaintTicketStatusEnum>(reader.GetString(reader.GetOrdinal("status")), ignoreCase: true);
            int categoryId = reader.GetInt32(reader.GetOrdinal("category_id"));
            int subcategoryId = reader.GetInt32(reader.GetOrdinal("subcategory_id"));
            string subject = reader.GetString(reader.GetOrdinal("subject"));
            string description = reader.GetString(reader.GetOrdinal("description"));
            DateTime creationTimestamp = reader.GetDateTime(reader.GetOrdinal("created_at"));
            ComplaintTicketUrgencyLevelEnum urgency = Enum.Parse<ComplaintTicketUrgencyLevelEnum>(reader.GetString(reader.GetOrdinal("urgency_level")), ignoreCase: true);

            ComplaintTicketCategory category = categoryRepository.GetById(categoryId);
            ComplaintTicketSubcategory subcategory = subcategoryRepository.GetById(subcategoryId);
            User creator = userRepository.GetById(userId);

            return new ComplaintTicket(ticketId, creator, status, category, subcategory, subject, description, creationTimestamp, urgency);
        }

        protected override int GetEntityId(ComplaintTicket ticketEntity)
        {
            return ticketEntity.TicketId;
        }
    }
}