using Dapper;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ThinkBooksWebsite.Controllers;

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

    public class AuthorRepository
    {
        public List<Author> GetAuthors()
        {
            using (var db = Util.GetOpenConnection())
            {
                return db.Query<Author>(@"
                   SELECT * FROM Author
                   ORDER BY LastName
                   ").ToList();
            }
        }
    }
}