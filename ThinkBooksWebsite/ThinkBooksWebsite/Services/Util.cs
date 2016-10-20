using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ThinkBooksWebsite.Services
{
    public static class Util
    {
        public static IDbConnection GetOpenConnection()
        {
            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ThinkBooksConnectionString"].ConnectionString);
            connection.Open();
            return connection;
        }
    }
}