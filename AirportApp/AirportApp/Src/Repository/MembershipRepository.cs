using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using AirportApp.Src.Domain;

namespace AirportApp.Src.Repository
{
    public class MembershipRepository : IMembershipRepository
    {
        private readonly IDatabaseConnectionFactory databaseConnectionFactory;

        public MembershipRepository(IDatabaseConnectionFactory databaseConnectionFactory)
        {
            this.databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public Membership? GetMembershipById(int id)
        {
            Membership? membership = null;
            using (var connection = databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                string query = "SELECT membership_id, name, flight_discount_percentage FROM Memberships WHERE membership_id = @MembershipId";

                using (var getMembershipByIdCommand = new SqlCommand(query, connection))
                {
                    getMembershipByIdCommand.Parameters.AddWithValue("@MembershipId", id);

                    using (var reader = getMembershipByIdCommand.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            membership = new Membership
                            {
                                MembershipId = reader.GetInt32(reader.GetOrdinal("membership_id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                FlightDiscountPercentage = (float)reader.GetByte(reader.GetOrdinal("flight_discount_percentage"))
                            };
                        }
                    }
                }
            }

            return membership;
        }

        public IEnumerable<Membership> GetAllMemberships()
        {
            var memberships = new List<Membership>();
            using (var connection = databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                string query = "SELECT membership_id, name, flight_discount_percentage FROM Memberships";

                using (var getAllMembershipsCommand = new SqlCommand(query, connection))
                using (var reader = getAllMembershipsCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        memberships.Add(new Membership
                        {
                            MembershipId = reader.GetInt32(reader.GetOrdinal("membership_id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            FlightDiscountPercentage = (float)reader.GetByte(reader.GetOrdinal("flight_discount_percentage"))
                        });
                    }
                }
            }
            return memberships;
        }

        public IEnumerable<MembershipAddonDiscount> GetAddonDiscounts(int membershipId)
        {
            var discounts = new List<MembershipAddonDiscount>();
            using (var connection = databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                string query = @"
                    SELECT mad.discount_percentage, 
                           m.membership_id, m.name as membership_name, m.flight_discount_percentage,
                           a.addon_id, a.name as addon_name, a.base_price
                    FROM Memberships_AddOns_Discounts mad
                    INNER JOIN Memberships m ON mad.membership_id = m.membership_id
                    INNER JOIN AddOns a ON mad.addon_id = a.addon_id
                    WHERE mad.membership_id = @MembershipId";

                using (var getAddOnDiscountsCommand = new SqlCommand(query, connection))
                {
                    getAddOnDiscountsCommand.Parameters.AddWithValue("@MembershipId", membershipId);

                    using (var reader = getAddOnDiscountsCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var membership = new Membership
                            {
                                MembershipId = reader.GetInt32(reader.GetOrdinal("membership_id")),
                                Name = reader.GetString(reader.GetOrdinal("membership_name")),
                                FlightDiscountPercentage = (float)reader.GetByte(reader.GetOrdinal("flight_discount_percentage"))
                            };

                            var addon = new AddOn
                            {
                                AddOnId = reader.GetInt32(reader.GetOrdinal("addon_id")),
                                Name = reader.GetString(reader.GetOrdinal("addon_name")),
                                BasePrice = (float)reader.GetDecimal(reader.GetOrdinal("base_price"))
                            };

                            var discount = new MembershipAddonDiscount(
                                membership,
                                addon,
                                (float)reader.GetByte(reader.GetOrdinal("discount_percentage")));

                            discounts.Add(discount);
                        }
                    }
                }
            }
            return discounts;
        }
    }
}
