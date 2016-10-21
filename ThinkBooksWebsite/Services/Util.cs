using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using StackExchange.Profiling;
using StackExchange.Profiling.Data;
using static System.String;

namespace ThinkBooksWebsite.Services
{
    public static class Util
    {
        //public static IDbConnection GetOpenConnection()
        //{
        //    var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ThinkBooksConnectionString"].ConnectionString);
        //    connection.Open();
        //    return connection;
        //}

        public static IDbConnection GetOpenConnection()
        {
            var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ThinkBooksConnectionString"].ConnectionString);
            connection.Open();
            MiniProfiler.Settings.SqlFormatter = new StackExchange.Profiling.SqlFormatters.SqlServerFormatter();
            return new ProfiledDbConnection(connection, MiniProfiler.Current);
        }

        public static string CapitaliseFirstLetter(this string s)
        {
            if (IsNullOrEmpty(s)) return s;
            if (s.Length == 1) return s.ToUpper();
            return s.Remove(1).ToUpper() + s.Substring(1);
        }
    }
}