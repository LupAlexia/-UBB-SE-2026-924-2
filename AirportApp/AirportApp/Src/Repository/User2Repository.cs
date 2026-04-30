using System;
using System.Linq;
using Microsoft.Data.SqlClient;
using AirportApp.Src.Domain;

namespace AirportApp.Src.Repository
{
    public class User2Repository : IUser2Repository
    {
        private readonly IDatabaseConnectionFactory databaseConnectionFactory;
        private readonly IMembershipRepository membershipRepository;

        public User2Repository(IDatabaseConnectionFactory databaseConnectionFactory, IMembershipRepository membershipRepository)
        {
            this.databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
            this.membershipRepository = membershipRepository ?? throw new ArgumentNullException(nameof(membershipRepository));
        }

        public User2? GetById(int id)
        {
            User2? user = null;
            using (var connection = this.databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT u.user_id, u.email, u.phone, u.username, u.password_hash, 
                           u.membership_id, m.name as membership_name, m.flight_discount_percentage
                    FROM Users u
                    LEFT JOIN Memberships m ON u.membership_id = m.membership_id
                    WHERE u.user_id = @UserId";

                using (var getUserByIdCommand = new SqlCommand(query, connection))
                {
                    getUserByIdCommand.Parameters.AddWithValue("@UserId", id);
                    using (var reader = getUserByIdCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = this.MapUser(reader);
                        }
                    }
                }
            }

            return user;
        }

        public User2? GetByEmail(string email)
        {
            User2? user = null;
            using (var connection = this.databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT u.user_id, u.email, u.phone, u.username, u.password_hash, 
                           u.membership_id, m.name as membership_name, m.flight_discount_percentage
                    FROM Users u
                    LEFT JOIN Memberships m ON u.membership_id = m.membership_id
                    WHERE u.email = @Email";

                using (var getUserByEmailCommand = new SqlCommand(query, connection))
                {
                    getUserByEmailCommand.Parameters.AddWithValue("@Email", email);
                    using (var reader = getUserByEmailCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = this.MapUser(reader);
                        }
                    }
                }
            }

            return user;
        }

        public void AddUser(User2 user)
        {
            using (var connection = this.databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO Users (email, phone, username, password_hash, membership_id) 
                    VALUES (@Email, @Phone, @Username, @PasswordHash, @MembershipId)";

                using (var insertUserCommand = new SqlCommand(query, connection))
                {
                    insertUserCommand.Parameters.AddWithValue("@Email", user.Email);
                    insertUserCommand.Parameters.AddWithValue("@Phone", user.Phone ?? (object)DBNull.Value);
                    insertUserCommand.Parameters.AddWithValue("@Username", user.Username);
                    insertUserCommand.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    insertUserCommand.Parameters.AddWithValue("@MembershipId", user.Membership?.MembershipId ?? (object)DBNull.Value);
                    insertUserCommand.ExecuteNonQuery();
                }
            }
        }

        public void UpdateUserMembership(int userId, int newMembershipId)
        {
            using (var connection = this.databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                string query = @"
                    UPDATE Users 
                    SET membership_id = @MembershipId
                    WHERE user_id = @UserId";

                using (var updateUserMembershipCommand = new SqlCommand(query, connection))
                {
                    updateUserMembershipCommand.Parameters.AddWithValue("@UserId", userId);
                    updateUserMembershipCommand.Parameters.AddWithValue("@MembershipId", newMembershipId);
                    updateUserMembershipCommand.ExecuteNonQuery();
                }
            }
        }

        private User2 MapUser(SqlDataReader reader)
        {
            int membershipIdOrdinal = reader.GetOrdinal("membership_id");
            Membership? membership = null;

            if (!reader.IsDBNull(membershipIdOrdinal))
            {
                membership = new Membership
                {
                    MembershipId = reader.GetInt32(membershipIdOrdinal),
                    Name = reader.GetString(reader.GetOrdinal("membership_name")),
                    FlightDiscountPercentage = (float)reader.GetByte(reader.GetOrdinal("flight_discount_percentage"))
                };

                membership.AddonDiscounts = this.membershipRepository.GetAddonDiscounts(membership.MembershipId).ToList();
            }

            return new User2(
                reader.GetInt32(reader.GetOrdinal("user_id")),
                reader.GetString(reader.GetOrdinal("email")),
                reader.IsDBNull(reader.GetOrdinal("phone")) ? null : reader.GetString(reader.GetOrdinal("phone")),
                reader.GetString(reader.GetOrdinal("username")),
                reader.GetString(reader.GetOrdinal("password_hash")),
                membership);
        }
    }
}
