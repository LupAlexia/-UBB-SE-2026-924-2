using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace AirportApp.Src.Repository
{
    public class DatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly string connectionString;

        public DatabaseConnectionFactory()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            connectionString = configuration.GetConnectionString("DefaultConnection")
                                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public DatabaseConnectionFactory(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}
