using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using ThinkBooksWebsite.Models;

namespace ThinkBooksWebsite.Services
{
    public class AuthorsRepository
    {
        // Stored Proc
        //public List<Author> GetAuthors(string sortColumnAndDirection)
        //{
        //    using (var db = Util.GetOpenConnection())
        //    {
        //        var p = new DynamicParameters();
        //        var sortDirection = "ASC";
        //        var sortColumn = sortColumnAndDirection;
        //        if (sortColumnAndDirection.EndsWith("_desc"))
        //        {
        //            sortDirection = "DESC";
        //            sortColumn = sortColumn.Substring(0, sortColumnAndDirection.Length - 5);
        //        }
        //        p.Add("@SortColumn", sortColumn);
        //        p.Add("@SortDirection", sortDirection);
        //        return db.Query<Author>("GetAuthors", p, commandType: CommandType.StoredProcedure).ToList();
        //    }
        //}

        // Using inline SQL
        public List<Author> GetAuthors(string sortColumnAndDirection)
        {
            using (var db = Util.GetOpenConnection())
            {
                var sortDirection = "ASC";
                var sortColumn = sortColumnAndDirection;
                if (sortColumnAndDirection.EndsWith("_desc"))
                {
                    sortDirection = "DESC";
                    sortColumn = sortColumn.Substring(0, sortColumnAndDirection.Length - 5);
                }

                // Making sure it can only be set to DESC or ASC
                string sanitizedSortDirection = sortDirection == "DESC" ? "DESC" : "ASC";

                DbCommandBuilder commandBuilder = new SqlCommandBuilder();
                string sanitizedSortColumn = commandBuilder.QuoteIdentifier(sortColumn);

                var sql = "SELECT * FROM Author WHERE FirstName LIKE CONCAT('%',@firstName,'%') ORDER BY " + sanitizedSortColumn + " " + sanitizedSortDirection;
                return db.Query<Author>(sql, new { firstName = "Ali"}).ToList();
            }
        }

        // {=} is inline value injection, but has to be an int, or enum
        //SELECT TOP {= count} *FROM Author WHERE FirstName LIKE { @firstName} ORDER BY {= sortOrder}

        // could do this but SQL injection issue!
        //"SELECT * FROM Author ORDER BY " + sortOrder

        // can you do SQL injection with a SP?
        //return db.Query<Author>("SELECT * FROM Author ORDER BY " +  sortOrder, new { sortOrder = AuthorSortDirection.LastName, sortDirection }).ToList();

    }
}