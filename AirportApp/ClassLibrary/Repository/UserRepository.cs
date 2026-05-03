using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;
using Microsoft.Data.SqlClient;

namespace AirportApp.ClassLibrary.Repository
{
    public class UserRepository : DatabaseRepository<int, User>, IRepository<int, User>, IUserRepository
    {
        public int CreateNewEntity(User userEntity)
        {
            if (userEntity == null)
            {
                throw new ArgumentNullException(nameof(userEntity), "User cannot be null.");
            }

            string insertQuery = "INSERT INTO [User] " +
                "(name, email) " +
                "OUTPUT INSERTED.user_id " +
                "VALUES (@name, @email)";

            SqlCommand insertCommand = new SqlCommand(insertQuery);

            insertCommand.Parameters.AddWithValue("@name", userEntity.RetrieveConfiguredDisplayFullNameForBot());
            insertCommand.Parameters.AddWithValue("@email", userEntity.RetrieveConfiguredEmailAddressForBotContact());

            int generatedIdentificationNumber = Add(insertCommand, userEntity);
            return generatedIdentificationNumber;
        }

        public void DeleteById(int identificationNumber)
        {
            string deleteQuery = "DELETE FROM [User] WHERE user_id = @id";
            SqlCommand deleteCommand = new SqlCommand(deleteQuery);
            deleteCommand.Parameters.AddWithValue("@id", identificationNumber);

            DeleteById(identificationNumber, deleteCommand);
        }

        public IEnumerable<User> GetAll()
        {
            string selectAllQuery = "SELECT * FROM [User]";
            SqlCommand getAllCommand = new SqlCommand(selectAllQuery);
            return GetAll(getAllCommand);
        }

        public User GetById(int identificationNumber)
        {
            string selectByIdQuery = "SELECT * FROM [User] WHERE user_id = @id";
            SqlCommand selectByIdCommand = new SqlCommand(selectByIdQuery);
            selectByIdCommand.Parameters.AddWithValue("@id", identificationNumber);

            User foundUser = GetById(identificationNumber, selectByIdCommand);

            if (foundUser == null)
            {
                throw new KeyNotFoundException($"User with id {identificationNumber} was not found.");
            }

            return foundUser;
        }

        public void UpdateById(int identificationNumber, User userEntity)
        {
            if (userEntity == null)
            {
                throw new ArgumentNullException(nameof(userEntity), "User cannot be null.");
            }

            string updateQuery = "UPDATE [User] SET " +
                "name = @name, " +
                "email = @email " +
                "WHERE user_id = @id";

            SqlCommand updateCommand = new SqlCommand(updateQuery);

            updateCommand.Parameters.AddWithValue("@id", identificationNumber);
            updateCommand.Parameters.AddWithValue("@name", userEntity.RetrieveConfiguredDisplayFullNameForBot());
            updateCommand.Parameters.AddWithValue("@email", userEntity.RetrieveConfiguredEmailAddressForBotContact());

            UpdateById(identificationNumber, updateCommand, userEntity);
        }

        protected override int GetEntityId(User userEntity)
        {
            return userEntity.UserId;
        }

        protected override User MapRowToEntity(SqlDataReader dataReader)
        {
            int userIdentificationNumber = dataReader.GetInt32(dataReader.GetOrdinal("user_id"));
            string userFullName = dataReader.GetString(dataReader.GetOrdinal("name"));
            string userEmailAddress = dataReader.GetString(dataReader.GetOrdinal("email"));

            return new User(userIdentificationNumber, userFullName, userEmailAddress);
        }
    }
}