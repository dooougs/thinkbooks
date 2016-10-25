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

namespace ThinkBooksWebsite.Services
{
    public class AuthorViewModel
    {
        public List<Author> Authors { get; set; }
        public int CountOfAuthors { get; set; }
    }

    public class AuthorsRepository
    {
        // Stored Proc
        public AuthorViewModel GetAuthors2(string sortColumnAndDirection, int? authorIDFilter,
            string firstNameFilter, string lastNameFilter, DateTime? dateOfBirthFilter, int numberOfResults)
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
                if (firstNameFilter == "")
                    p.Add("@FirstName");
                else
                    p.Add("@FirstName", firstNameFilter);

                if (lastNameFilter == "")
                    p.Add("@LastName");
                else
                    p.Add("@LastName", lastNameFilter);

                p.Add("@DateOfBirth", dateOfBirthFilter);
                p.Add("@NumberOfResults", numberOfResults);

                // keep the main query, and count logic in 1 SP which returns 2 record sets (the results, and a single row - count)
                var x = db.QueryMultiple("GetAuthors", p, commandType: CommandType.StoredProcedure);
                var result = x.Read<Author>().ToList();
                var count = x.Read<int>().Single();
                var vm = new AuthorViewModel
                {
                    Authors = result,
                    CountOfAuthors = count
                };
                return vm;
            }
        }

        // Using inline SQL
        public AuthorViewModel GetAuthors(string sortColumnAndDirection, int? authorIDFilter,
            string firstNameFilter, string lastNameFilter, DateTime? dateOfBirthFilter, int numberOfResults)
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
                    SELECT TOP (@NumberOfResults) * FROM Author 
                    WHERE (@AuthorID IS NULL OR AuthorID = @AuthorID)
                    AND (@FirstName IS NULL OR FirstName LIKE CONCAT('%',@FirstName,'%'))
	                AND (@LastName IS NULL OR LastName LIKE '%'+@LastName+'%')
	                AND (@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)
                    ORDER BY " + sanitizedSortColumn + " " + sortDirection;

                var result = db.Query<Author>(sql,
                    new
                    {
                        AuthorID = authorIDFilter,
                        FirstName = firstNameFilter,
                        LastName = lastNameFilter,
                        DateOfBirth = dateOfBirthFilter,
                        numberOfResults
                    }).ToList();

                // seems super fast doing 2 queries ie don't need to do return multiple record sets
                string sqlCount;
                if (authorIDFilter == null && dateOfBirthFilter == null && firstNameFilter == "" && lastNameFilter == "")
                {
                    sqlCount = @"SELECT SUM(p.rows)
		                        FROM sys.partitions AS p
		                        INNER JOIN sys.tables AS t
		                        ON p.[object_id] = t.[object_id]
		                        INNER JOIN sys.schemas AS s
		                        ON t.[schema_id] = s.[schema_id]
		                        WHERE p.index_id IN (0,1) -- heap or clustered index
		                        AND t.name = N'Author'
		                        AND s.name = N'dbo'";
                }
                else
                {
                    sqlCount = @"SELECT COUNT(*) FROM Author
                                WHERE(@AuthorID IS NULL OR AuthorID = @AuthorID)
                                AND(@firstName IS NULL OR FirstName LIKE CONCAT('%', @firstName, '%'))
                                AND(@LastName IS NULL OR LastName LIKE '%' + @LastName + '%')
                                AND(@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)";
                }
                var count = db.Query<int>(sqlCount, new
                {
                    authorID = authorIDFilter,
                    firstName = firstNameFilter,
                    lastName = lastNameFilter,
                    dateOfBirth = dateOfBirthFilter
                }).Single();

                var vm = new AuthorViewModel
                {
                    Authors = result,
                    CountOfAuthors = count
                };

                return vm;
            }
        }

        // {=} is inline value injection, but has to be an int, or enum
        //SELECT TOP {= count} *FROM Author WHERE FirstName LIKE { @firstName} ORDER BY {= sortOrder}

        // could do this but SQL injection issue!
        //"SELECT * FROM Author ORDER BY " + sortOrder

        // can you do SQL injection with a SP?
        //return db.Query<Author>("SELECT * FROM Author ORDER BY " +  sortOrder, new { sortOrder = AuthorSortDirection.LastName, sortDirection }).ToList();

        public void LoadDataSqlBulkCopyAuthorsBooks()
        {
            var numberInBatch = 10000;
            var numberOfBatches = 25;

            TruncateTable();

            var firstnames = LoadFirstNames();
            var surnames = LoadSurnames();
            var words = LoadWords();

            var dataTablesAuthor = new List<DataTable>();
            for (var i = 0; i < numberOfBatches; i++)
                dataTablesAuthor.Add(BuildDataTableAuthors(i * numberInBatch, numberInBatch, firstnames, surnames, words));

            // Add Authors
            foreach (var dt in dataTablesAuthor)
            {
                using (var s = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["ThinkBooksConnectionString"].ConnectionString))
                {
                    s.DestinationTableName = "Author";
                    s.BatchSize = 0;
                    s.WriteToServer(dt);
                    s.Close();
                }
            }

            //var dataTablesBooks = new List<DataTable>();
            //for (var i = 0; i < numberOfBatches; i++)
            //{
            //    dataTablesBooks.Add(BuildDataTableBooks(i * numberInBatch, numberInBatch, words));
            //}
            //// Add Books - we know the range of AuthorID's, so can assign books appropriately
            //foreach (var dt in dataTablesBooks)
            //{
            //    using (var s = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["BookTechConnectionString"].ConnectionString))
            //    {
            //        s.DestinationTableName = "Books";
            //        s.BatchSize = 0;
            //        s.WriteToServer(dt);
            //        s.Close();
            //    }
            //}

            //using (var c = Util.GetOpenConnection())
            //{
            //    c.Execute("ALTER TABLE[dbo].[Books] WITH CHECK ADD CONSTRAINT[FK_Books_ToAuthor] FOREIGN KEY([AuthorID]) REFERENCES[dbo].[Authors]([AuthorID])");
            //}
        }

        private static List<string> LoadWords()
        {
            var p3 = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "words.txt");
            var words = File.ReadAllLines(p3).ToList();
            return words;
        }

        private static List<string> LoadSurnames()
        {
            var p2 = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "surnames.txt");
            var surnames = File.ReadAllLines(p2).ToList();
            return surnames;
        }

        private static List<string> LoadFirstNames()
        {
            var p1 = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "firstnames.txt");
            var firstnames = File.ReadAllLines(p1).ToList();
            return firstnames;
        }

        private static void TruncateTable()
        {
            using (var c = Util.GetOpenConnection())
            {
                //try
                //{
                //    c.Execute("ALTER TABLE [dbo].[Books] DROP CONSTRAINT [FK_Books_ToAuthor]");
                //}
                //catch
                //{
                //}

                c.Execute("TRUNCATE TABLE Author;");
            }
        }

        //private DataTable BuildDataTableBooks(int from, int count, List<string> words)
        //{
        //    var dt = new DataTable();
        //    dt.Columns.Add("BookID", typeof(int));
        //    dt.Columns.Add("Title", typeof(string));
        //    dt.Columns.Add("AuthorID", typeof(int));
        //    int counter = 1;
        //    for (int i = from; i < from + count; i++)
        //    {
        //        List<Book> books = MakeRandomNumberOfBooks(words);
        //        foreach (var book in books)
        //        {
        //            DataRow row = dt.NewRow();
        //            row.ItemArray = new object[] { counter, book.Title, i + 1 };
        //            dt.Rows.Add(row);
        //            counter++;
        //        }
        //    }
        //    return dt;
        //}

        public DataTable BuildDataTableAuthors(int from, int count, List<string> firstnames, List<string> surnames, List<string> words)
        {
            var dt = new DataTable();
            dt.Columns.Add("AuthorID", typeof(int));
            dt.Columns.Add("FirstName", typeof(string));
            dt.Columns.Add("LastName", typeof(string));
            dt.Columns.Add("DateOfBirth", typeof(DateTime));
            //dt.Columns.Add("EmailAddress", typeof(string));

            for (int i = from; i < from + count; i++)
            {
                Author author = MakeRandomAuthor(firstnames, surnames);
                DataRow row = dt.NewRow();
                // this looks like AuthorID initially will be 0, however it is 1 (identity).. so its just a key for the dt?
                //row.ItemArray = new object[] { i, author.FirstName, author.LastName, author.EmailAddress };
                row.ItemArray = new object[] { i, author.FirstName, author.LastName, author.DateOfBirth };
                dt.Rows.Add(row);
            }
            return dt;
        }

        public Author MakeRandomAuthor(List<string> firstnames, List<string> surnames)
        {
            var r = GetRandom();
            string firstname = firstnames[r.Next(firstnames.Count)].CapitaliseFirstLetter();
            string surname = surnames[r.Next(surnames.Count)].CapitaliseFirstLetter();
            var year = r.Next(1930, 2010);
            var month = r.Next(1, 13);
            var day = r.Next(1, 28);
            DateTime dateOfBirth = new DateTime(year, month, day);
            //string email = firstname + "@" + surname + ".com";
            //return new Author { FirstName = firstname, LastName = surname, EmailAddress = email };
            return new Author { FirstName = firstname, LastName = surname, DateOfBirth = dateOfBirth };
        }

        // To stop similar random numbers use the same Random instance
        private static Random rnd;
        public static Random GetRandom()
        {
            if (rnd != null) return rnd;
            rnd = new Random();
            return rnd;
        }



        //public List<Book> MakeRandomNumberOfBooks(List<string> words)
        //{
        //    var books = new List<Book>();
        //    for (int j = 0; j < Util.GetRandom().Next(1, 5); j++)
        //    {
        //        Book book = GetBookTitle(words);
        //        books.Add(book);
        //    }
        //    return books;
        //}

        //public Book GetBookTitle(List<string> words)
        //{
        //    var r = Util.GetRandom();
        //    int firstWordIndex = r.Next(words.Count);
        //    string firstWord = words[firstWordIndex].CapitaliseFirstLetter();

        //    // unless want to run out of words!
        //    //words.RemoveAt(firstWordIndex);

        //    int secondWordIndex = r.Next(words.Count);
        //    string secondWord = words[secondWordIndex].CapitaliseFirstLetter();

        //    var book = new Book { Title = firstWord + " of the " + secondWord };
        //    return book; //xx
        //}

    }
}