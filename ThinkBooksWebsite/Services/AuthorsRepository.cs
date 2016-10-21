using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using ThinkBooksWebsite.Models;
using System;

namespace ThinkBooksWebsite.Services
{
    public class AuthorsRepository
    {
        // Stored Proc
        public List<Author> GetAuthors2(string sortColumnAndDirection, int? authorIDFilter, string firstNameFilter, string lastNameFilter, DateTime? dateOfBirthFilter)
        {
            using (var db = Util.GetOpenConnection())
            {
                var p = new DynamicParameters();
                var sortDirection = "ASC";
                var sortColumn = sortColumnAndDirection;
                if (sortColumnAndDirection.EndsWith("_desc"))
                {
                    sortDirection = "DESC";
                    sortColumn = sortColumn.Substring(0, sortColumnAndDirection.Length - 5);
                }
                p.Add("@SortColumn", sortColumn);
                p.Add("@SortDirection", sortDirection);
                p.Add("@AuthorID", authorIDFilter);
                p.Add("@FirstName", firstNameFilter);
                p.Add("@LastName", lastNameFilter);
                p.Add("@DateOfBirth", dateOfBirthFilter);
                return db.Query<Author>("GetAuthors", p, commandType: CommandType.StoredProcedure).ToList();
            }
        }

        // Using inline SQL
        public List<Author> GetAuthors(string sortColumnAndDirection, int? authorIDFilter, string firstNameFilter, string lastNameFilter, DateTime? dateOfBirthFilter)
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

                var commandBuilder = new SqlCommandBuilder();
                var sanitizedSortColumn = commandBuilder.QuoteIdentifier(sortColumn);

                var sql = @"
                    SELECT * FROM Author 
                    WHERE (@AuthorID IS NULL OR AuthorID = @AuthorID)
                    AND (@firstName IS NULL OR FirstName LIKE CONCAT('%',@firstName,'%'))
	                AND (@LastName IS NULL OR LastName LIKE '%'+@LastName+'%')
	                AND (@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)
                    ORDER BY " + sanitizedSortColumn + " " + sortDirection;

                return db.Query<Author>(sql, new { authorID = authorIDFilter, firstName = firstNameFilter, lastName = lastNameFilter, dateOfBirth = dateOfBirthFilter }).ToList();
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