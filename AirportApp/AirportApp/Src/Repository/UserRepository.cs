using AirportApp.Src.Domain;
using AirportApp.Src.Model;
using AirportApp.Src.Repository.Database;
using AirportApp.Src.Repository.Interfaces;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace AirportApp.Src.Repository
{
    public class UserRepository : DatabaseRepository<int, User>, IRepository<int, User>, IUserRepository
    {
        private readonly IMembershipRepository membershipRepository;

        public UserRepository(IMembershipRepository membershipRepository) 
        {
            this.membershipRepository = membershipRepository ?? throw new ArgumentNullException(nameof(membershipRepository), "Membership repository cannot be null.");
        }

        public int CreateNewEntity(User userEntity)
        {
            if (userEntity == null)
            {
                throw new ArgumentNullException(nameof(userEntity), "User cannot be null.");
            }

            string insertQuery = "INSERT INTO [User] (name, email, membership_id) " +
                                 "OUTPUT INSERTED.user_id " +
                                 "VALUES (@name, @email, @membershipId)";

            SqlCommand insertCommand = new SqlCommand(insertQuery);

            insertCommand.Parameters.AddWithValue("@name", userEntity.RetrieveConfiguredDisplayFullNameForBot());
            insertCommand.Parameters.AddWithValue("@email", userEntity.RetrieveConfiguredEmailAddressForBotContact());
            insertCommand.Parameters.AddWithValue("@membershipId", (object?)userEntity.Membership?.MembershipId ?? DBNull.Value);

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

            int membershipIdOrdinal = dataReader.GetOrdinal("membership_id");
            Membership? membership = null;

            if (!dataReader.IsDBNull(membershipIdOrdinal))
            {
                membership = new Membership
                {
                    MembershipId = dataReader.GetInt32(membershipIdOrdinal),
                    Name = dataReader.GetString(dataReader.GetOrdinal("membership_name")),
                    FlightDiscountPercentage = (float)dataReader.GetByte(dataReader.GetOrdinal("flight_discount_percentage"))
                };

                membership.AddonDiscounts = this.membershipRepository.GetAddonDiscounts(membership.MembershipId).ToList();
            }

            int userIdentificationNumber = dataReader.GetInt32(dataReader.GetOrdinal("user_id"));
            string userFullName = dataReader.GetString(dataReader.GetOrdinal("name"));
            string userEmailAddress = dataReader.GetString(dataReader.GetOrdinal("email"));


            return new User(userIdentificationNumber, userFullName, userEmailAddress, membership);
        }

        public IEnumerable<User> GetAllMemberships()
        {
            string selectAllQuery = @"
                SELECT u.*, m.name as membership_name, m.flight_discount_percentage 
                FROM [User] u
                LEFT JOIN [Membership] m ON u.membership_id = m.membership_id";

            SqlCommand getAllCommand = new SqlCommand(selectAllQuery);
            return GetAll(getAllCommand);
        }

        public User? GetByIdWithMembership(int id)
        {
            User? user = null;
            
                string query = @"
                    SELECT u.user_id,u.name, u.email,
                           u.membership_id, m.name as membership_name, m.flight_discount_percentage
                    FROM Users u
                    LEFT JOIN Memberships m ON u.membership_id = m.membership_id
                    WHERE u.user_id = @UserId";

                using (var getUserByIdCommand = new SqlCommand(query))
                {
                    getUserByIdCommand.Parameters.AddWithValue("@UserId", id);
                    using (var reader = getUserByIdCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = this.MapRowToEntity(reader);
                        }
                    }
                }
            return user;
        }

        public void AddUserWithMembership(User user)
        {
            
                string query = @"
                    INSERT INTO Users (name, email, membership_id) 
                    VALUES (@Name, @Email, @MembershipId)";

                using (var insertUserCommand = new SqlCommand(query))
                {
                insertUserCommand.Parameters.AddWithValue("@Name", user.RetrieveConfiguredDisplayFullNameForBot());
                insertUserCommand.Parameters.AddWithValue("@Email", user.RetrieveConfiguredEmailAddressForBotContact());

                insertUserCommand.Parameters.AddWithValue("@MembershipId", (object?)user.Membership?.MembershipId ?? DBNull.Value);
                insertUserCommand.ExecuteNonQuery();
            }
        }

        public void UpdateUserMembership(int userId, int newMembershipId)
        {
            
                string query = @"
                    UPDATE Users 
                    SET membership_id = @MembershipId
                    WHERE user_id = @UserId";

                using (var updateUserMembershipCommand = new SqlCommand(query))
                {
                    updateUserMembershipCommand.Parameters.AddWithValue("@UserId", userId);
                    updateUserMembershipCommand.Parameters.AddWithValue("@MembershipId", newMembershipId);
                    updateUserMembershipCommand.ExecuteNonQuery();
                }
            
        }
    }
}
