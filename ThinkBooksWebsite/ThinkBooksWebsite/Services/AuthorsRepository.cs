using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using ThinkBooksWebsite.Models;

namespace ThinkBooksWebsite.Services
{
    //public enum AuthorSortDirection
    //{
    //    AuthorID,
    //    FirstName, 
    //    LastName
    //}

    public class AuthorsRepository
    {
        public List<Author> GetAuthors(string sortColumn, string sortDirection)
        {
            using (var db = Util.GetOpenConnection())
            {
                // {=} is inline value injection, but has to be an int, or enum
                //SELECT TOP {= count} *FROM Author WHERE FirstName LIKE { @firstName} ORDER BY {= sortOrder}

                // could do this but SQL injection issue!
                //"SELECT * FROM Author ORDER BY " + sortOrder

                // can you do SQL injection with a SP?
                //return db.Query<Author>("SELECT * FROM Author ORDER BY " +  sortOrder, new { sortOrder = AuthorSortDirection.LastName, sortDirection }).ToList();

                var p = new DynamicParameters();
                p.Add("@SortColumn", sortColumn);
                p.Add("@SortDirection", sortDirection);
                return db.Query<Author>("GetAuthors", p, commandType: CommandType.StoredProcedure).ToList();
            }
        }
    }
}