using Microsoft.Data.SqlClient;

namespace AirportApp.Src.Repository
{
    public interface IDatabaseConnectionFactory
    {
        SqlConnection GetConnection();
    }
}
