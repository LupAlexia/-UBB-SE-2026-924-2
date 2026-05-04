using Microsoft.Data.SqlClient;
using TicketManager.Repository;

namespace TicketManager.Tests.Integration;

public abstract class BaseIntegrationTest
{
    protected string GetTestConnectionString()
    {
        return "Server=MARINELA\\SQLEXPRESS01;Database=TicketsDB;Trusted_Connection=True;TrustServerCertificate=True;";
    }

    protected int GetFirstAvailableFlightId()
    {
        using var connection = new SqlConnection(GetTestConnectionString());
        connection.Open();

        using var getTopFlightIdCommand = new SqlCommand("SELECT TOP 1 id FROM Flights ORDER BY CASE WHEN date > GETDATE() THEN 0 ELSE 1 END, date ASC", connection);
        var scalarResult = getTopFlightIdCommand.ExecuteScalar();
        if (scalarResult == null || scalarResult == DBNull.Value)
        {
            throw new Exception("Nu s-au gasit zboruri in baza de date. Va rugam sa rulati scriptul de seed.");
        }

        return Convert.ToInt32(scalarResult);
    }
}



