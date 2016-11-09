using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using ThinkBooksWebsiteTesting.Models;

namespace ThinkBooksWebsiteTesting.Services
{
    public class AuthorsViewModel
    {
        public List<Author> Authors { get; set; }
        public int CountOfAuthors { get; set; }
    }

    public class AuthorsRepository
    {
        // Using inline SQL
        public AuthorsViewModel GetAuthors(string sortColumnAndDirection, int? authorIDFilter,
            string firstNameFilter, string lastNameFilter, DateTime? dateOfBirthFilter, int numberOfResults, int currentPage)
        {
            using (var db = Util.GetOpenConnection())
            {
                var result = db.GetAll<Author>().Take(20).ToList();


                //var sortDirection = "ASC";
                //var sortColumn = sortColumnAndDirection;
                //if (sortColumnAndDirection.EndsWith("_desc"))
                //{
                //    sortDirection = "DESC";
                //    sortColumn = sortColumn.Substring(0, sortColumnAndDirection.Length - 5);
                //}

                //var commandBuilder = new SqlCommandBuilder();
                //var sanitizedSortColumn = commandBuilder.QuoteIdentifier(sortColumn);

                //var offset = (currentPage - 1)*numberOfResults;

                //if (firstNameFilter == "") firstNameFilter = null;
                //if (lastNameFilter == "") lastNameFilter = null;

                //var sql = @"
                //    SELECT * FROM Author 
                //    WHERE (@AuthorID IS NULL OR AuthorID = @AuthorID)
                //    AND (@FirstName IS NULL OR FirstName LIKE CONCAT(@FirstName,'%'))
                //    AND (@LastName IS NULL OR LastName LIKE CONCAT(@LastName,'%'))
                //    AND (@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)
                //    ORDER BY " + sanitizedSortColumn + " " + sortDirection + @"
                //    OFFSET "+ offset + @" ROWS 
                //    FETCH NEXT " + numberOfResults + " ROWS ONLY";

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
                //                AND(@firstName IS NULL OR FirstName LIKE CONCAT(@firstName, '%'))
                //                AND (@LastName IS NULL OR LastName LIKE CONCAT(@LastName,'%'))
                //                AND(@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)";
                //}
                //var count = db.Query<int>(sqlCount, new
                //{
                //    authorID = authorIDFilter,
                //    firstName = firstNameFilter,
                //    lastName = lastNameFilter,
                //    dateOfBirth = dateOfBirthFilter
                //}).Single();

                var vm = new AuthorsViewModel
                {
                    Authors = result,
                    CountOfAuthors = 0
                };

                return vm;
            }
        }
    }
}