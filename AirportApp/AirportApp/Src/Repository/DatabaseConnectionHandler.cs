using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using DotNetEnv;
using Microsoft.Data.SqlClient;

namespace AirportApp.Src.Repository.Database
{
    public class DatabaseConnectionHandler
    {
        private static readonly DatabaseConnectionHandler DatabaseInstance = new DatabaseConnectionHandler();
        public static DatabaseConnectionHandler Instance => DatabaseInstance;

        private readonly string connectionString;

        public SqlConnection CreateConnection() => new SqlConnection(connectionString);

        private DatabaseConnectionHandler()
        {
            connectionString = InitializeConnectionString();
        }

        private string InitializeConnectionString()
        {
            // Încarcă variabilele din fișierul .env
            Env.Load();

            string serverAddress = Env.GetString("DB_SERVER"); // Ex: localhost\SQLEXPRESS
            string databaseName = Env.GetString("DB_NAME");   // Ex: CloudSpritzers
            string userName = Env.GetString("DB_USER");       // Ex: Iulia
            string userPassword = Env.GetString("DB_PASS");   // Ex: none

            if (string.IsNullOrEmpty(serverAddress) || string.IsNullOrEmpty(databaseName))
            {
                throw new Exception("Configurația bazei de date (Server/Name) lipsește din .env");
            }

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            builder.DataSource = serverAddress;
            builder.InitialCatalog = databaseName;
            builder.TrustServerCertificate = true; // Necesar pentru instanțe locale SQLEXPRESS

            // Verificăm dacă folosim Windows Authentication sau SQL Authentication
            if (userPassword.Equals("none", StringComparison.OrdinalIgnoreCase) || string.IsNullOrEmpty(userPassword))
            {
                // Windows Authentication (nu e nevoie de user/pass)
                builder.IntegratedSecurity = true;
            }
            else
            {
                // SQL Server Authentication
                builder.UserID = userName;
                builder.Password = userPassword;
                builder.IntegratedSecurity = false;
            }

            return builder.ConnectionString;
        }
    }
}