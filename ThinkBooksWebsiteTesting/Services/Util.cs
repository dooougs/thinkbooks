using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace ThinkBooksWebsiteTesting.Services
{
    public static class Util
    {
        public static IDbConnection GetOpenConnection()
        {
            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ThinkBooksConnectionString"].ConnectionString);
            connection.Open();
            return connection;
            //MiniProfiler.Settings.SqlFormatter = new StackExchange.Profiling.SqlFormatters.SqlServerFormatter();
            //return new ProfiledDbConnection(connection, MiniProfiler.Current);
        }
    }
}