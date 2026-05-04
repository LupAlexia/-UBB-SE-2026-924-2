using Microsoft.Data.SqlClient;

namespace AirportApp.ClassLibrary.Repository.Interfaces
{
    public interface IDatabaseConnectionFactory
    {
        SqlConnection GetConnection();
    }
}
