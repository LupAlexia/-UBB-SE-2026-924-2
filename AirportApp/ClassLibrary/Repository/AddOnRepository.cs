using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using AirportApp.ClassLibrary.Entity.Domain;
using AirportApp.ClassLibrary.Repository.Interfaces;

namespace AirportApp.ClassLibrary.Repository
{
    public class AddOnRepository : IAddOnRepository
    {
        private readonly IDatabaseConnectionFactory databaseConnectionFactory;

        public AddOnRepository(IDatabaseConnectionFactory databaseConnectionFactory)
        {
            this.databaseConnectionFactory = databaseConnectionFactory ?? throw new ArgumentNullException(nameof(databaseConnectionFactory));
        }

        public IEnumerable<AddOn> GetAllAddOns()
        {
            var addons = new List<AddOn>();
            using (var connection = databaseConnectionFactory.GetConnection())
            {
                connection.Open();
                string query = "SELECT addon_id, name, base_price FROM AddOns";

                using (var getAllAddOnsCommand = new SqlCommand(query, connection))
                using (var reader = getAllAddOnsCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        addons.Add(new AddOn
                        {
                            AddOnId = reader.GetInt32(reader.GetOrdinal("addon_id")),
                            Name = reader.GetString(reader.GetOrdinal("name")),
                            BasePrice = (float)reader.GetDecimal(reader.GetOrdinal("base_price"))
                        });
                    }
                }
            }

            return addons;
        }

        public IEnumerable<AddOn> GetAddOnsByIds(IEnumerable<int> ids)
        {
            var addons = new List<AddOn>();

            if (ids == null || !ids.Any())
            {
                return addons;
            }

            using (var connection = databaseConnectionFactory.GetConnection())
            {
                connection.Open();

                var parameters = ids.Select((identifier, index) => new { ParameterName = $"@Id{index}", Value = identifier }).ToList();
                string inClause = string.Join(", ", parameters.Select(parameter => parameter.ParameterName));

                string query = $"SELECT addon_id, name, base_price FROM AddOns WHERE addon_id IN ({inClause})";

                using (var getAddOnsByIdsCommand = new SqlCommand(query, connection))
                {
                    foreach (var parameter in parameters)
                    {
                        getAddOnsByIdsCommand.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
                    }

                    using (var reader = getAddOnsByIdsCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            addons.Add(new AddOn
                            {
                                AddOnId = reader.GetInt32(reader.GetOrdinal("addon_id")),
                                Name = reader.GetString(reader.GetOrdinal("name")),
                                BasePrice = (float)reader.GetDecimal(reader.GetOrdinal("base_price"))
                            });
                        }
                    }
                }
            }

            return addons;
        }
    }
}

