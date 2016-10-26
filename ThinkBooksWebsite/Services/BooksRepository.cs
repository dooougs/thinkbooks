using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using ThinkBooksWebsite.Models;
using System;
using System.Configuration;
using System.IO;
using System.Web.Hosting;
using Microsoft.Ajax.Utilities;

namespace ThinkBooksWebsite.Services
{
    public class BooksViewModel
    {
        public List<Book> Books { get; set; }
        public int CountOfBooks { get; set; }
    }

    public class BooksRepository
    {
        //    public BooksViewModel GetBooks(string sortColumnAndDirection, int numberOfResults, int currentPage, int? bookIDFilter,
        //        string titleFilter)
        public BooksViewModel GetBooks()
        {
            using (var db = Util.GetOpenConnection())
            {
                //var sortDirection = "ASC";
                //var sortColumn = sortColumnAndDirection;
                //if (sortColumnAndDirection.EndsWith("_desc"))
                //{
                //    sortDirection = "DESC";
                //    sortColumn = sortColumn.Substring(0, sortColumnAndDirection.Length - 5);
                //}

                //var commandBuilder = new SqlCommandBuilder();
                //var sanitizedSortColumn = commandBuilder.QuoteIdentifier(sortColumn);

                //var offset = (currentPage - 1) * numberOfResults;
                var sql = "SELECT top 10 * FROM Book";
                //var sql = @"
                //    SELECT * FROM Author 
                //    WHERE (@AuthorID IS NULL OR AuthorID = @AuthorID)
                //    AND (@FirstName IS NULL OR FirstName LIKE CONCAT('%',@FirstName,'%'))
                //    AND (@LastName IS NULL OR LastName LIKE '%'+@LastName+'%')
                //    AND (@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)
                //    ORDER BY " + sanitizedSortColumn + " " + sortDirection + @"
                //    OFFSET "+ offset + @" ROWS 
                //    FETCH NEXT " + numberOfResults + " ROWS ONLY";
                var result = db.Query<Book>(sql).ToList();
                //var result = db.Query<Author>(sql,
                //         new
                //         {
                //             AuthorID = authorIDFilter,
                //             FirstName = firstNameFilter,
                //             LastName = lastNameFilter,
                //             DateOfBirth = dateOfBirthFilter,
                //             numberOfResults,
                //             PageNumber = currentPage
                //         }).ToList();


                //// seems super fast doing 2 queries ie don't need to do return multiple record sets
                //string sqlCount;
                //if (authorIDFilter == null && dateOfBirthFilter == null && firstNameFilter.IsNullOrWhiteSpace() && lastNameFilter.IsNullOrWhiteSpace())
                //{
                //    sqlCount = @"SELECT SUM(p.rows)
                //          FROM sys.partitions AS p
                //          INNER JOIN sys.tables AS t
                //          ON p.[object_id] = t.[object_id]
                //          INNER JOIN sys.schemas AS s
                //          ON t.[schema_id] = s.[schema_id]
                //          WHERE p.index_id IN (0,1) -- heap or clustered index
                //          AND t.name = N'Author'
                //          AND s.name = N'dbo'";
                //}
                //else
                //{
                //    sqlCount = @"SELECT COUNT(*) FROM Author
                //                WHERE(@AuthorID IS NULL OR AuthorID = @AuthorID)
                //                AND(@firstName IS NULL OR FirstName LIKE CONCAT('%', @firstName, '%'))
                //                AND(@LastName IS NULL OR LastName LIKE '%' + @LastName + '%')
                //                AND(@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)";
                //}
                //var count = db.Query<int>(sqlCount, new
                //{
                //    authorID = authorIDFilter,
                //    firstName = firstNameFilter,
                //    lastName = lastNameFilter,
                //    dateOfBirth = dateOfBirthFilter
                //}).Single();

                var vm = new BooksViewModel
                {
                    Books = result,
                    CountOfBooks = 0
                };

                return vm;
            }
        }
    }
}