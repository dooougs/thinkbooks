using Dapper;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using ThinkBooksWebsite.Models;

namespace ThinkBooksWebsite.Services
{
    public class AuthorsViewModel
    {
        public List<Author> Authors { get; set; }
        public int CountOfAuthors { get; set; }
    }

    public class AuthorsRepository
    {
        public AuthorsViewModel GetAuthors(string sortColumnAndDirection, int? authorIDFilter,
            string firstNameFilter, string lastNameFilter, DateTime? dateOfBirthFilter, int numberOfResults, int currentPage)
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

                var offset = (currentPage - 1) * numberOfResults;

                // the MVC mapper will map an empty string from the firstName and lastName form post to an empty string
                // we depend on nulls in the SQL.  Int and DateTime are okay.
                if (firstNameFilter == "") firstNameFilter = null;
                if (lastNameFilter == "") lastNameFilter = null;

                if (sanitizedSortColumn == "[AuthorStatus]") sanitizedSortColumn = "[AuthorStatusName]";

                // note % preceding a like search is much slower (250ms to 65ms) and seems a rarer business case to do that
                var sql = @"
                        SELECT a.*, s.Name AS AuthorStatusName FROM Author a
                        INNER JOIN AuthorStatus s ON a.AuthorStatusID = s.AuthorStatusID
                        WHERE (@AuthorID IS NULL OR AuthorID = @AuthorID)
                        AND (@FirstName IS NULL OR FirstName LIKE CONCAT(@FirstName,'%'))
                        AND (@LastName IS NULL OR LastName LIKE CONCAT(@LastName,'%'))
                        AND (@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)
                        ORDER BY " + sanitizedSortColumn + " " + sortDirection + @"
                        OFFSET " + offset + @" ROWS 
                        FETCH NEXT " + numberOfResults + " ROWS ONLY";

                var result = db.Query<Author>(sql,
                         new
                         {
                             AuthorID = authorIDFilter,
                             FirstName = firstNameFilter,
                             LastName = lastNameFilter,
                             DateOfBirth = dateOfBirthFilter,
                             numberOfResults,
                             PageNumber = currentPage
                         }).ToList();


                // super fast doing 2 queries ie don't need to do return multiple record sets
                string sqlCount;
                if (authorIDFilter == null && dateOfBirthFilter == null && firstNameFilter.IsNullOrWhiteSpace() && lastNameFilter.IsNullOrWhiteSpace())
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
                                AND(@firstName IS NULL OR FirstName LIKE CONCAT(@firstName, '%'))
                                AND(@LastName IS NULL OR LastName LIKE CONCAT(@LastName,'%'))
                                AND(@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)";
                }
                var count = db.Query<int>(sqlCount, new
                {
                    authorID = authorIDFilter,
                    firstName = firstNameFilter,
                    lastName = lastNameFilter,
                    dateOfBirth = dateOfBirthFilter
                }).Single();

                var vm = new AuthorsViewModel
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

            TruncateTables();

            // Add AuthorStatus table
            using (var c = Util.GetOpenConnection())
            {
                var sql = @"SET IDENTITY_INSERT [dbo].[AuthorStatus] ON 
                            INSERT [dbo].[AuthorStatus] ([AuthorStatusID], [Name]) VALUES (1, N'Alive')
                            INSERT [dbo].[AuthorStatus] ([AuthorStatusID], [Name]) VALUES (2, N'Dead')
                            SET IDENTITY_INSERT [dbo].[AuthorStatus] OFF";
                c.Execute(sql);
            }


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

            var dataTablesBooks = new List<DataTable>();
            for (var i = 0; i < numberOfBatches; i++)
            {
                dataTablesBooks.Add(BuildDataTableBooks(i * numberInBatch, numberInBatch, words));
            }
            // Add Books - we know the range of AuthorID's, so can assign books appropriately
            foreach (var dt in dataTablesBooks)
            {
                using (var s = new SqlBulkCopy(ConfigurationManager.ConnectionStrings["ThinkBooksConnectionString"].ConnectionString))
                {
                    s.DestinationTableName = "Book";
                    s.BatchSize = 0;
                    s.WriteToServer(dt);
                    s.Close();
                }
            }

            using (var c = Util.GetOpenConnection())
            {
                c.Execute("ALTER TABLE[dbo].[Book] WITH CHECK ADD CONSTRAINT[FK_Book_Author] FOREIGN KEY([AuthorID]) REFERENCES[dbo].[Author]([AuthorID])");
                c.Execute("ALTER TABLE[dbo].[Author] WITH CHECK ADD CONSTRAINT[FK_Author_AuthorStatus] FOREIGN KEY([AuthorStatusID]) REFERENCES[dbo].[AuthorStatus]([AuthorStatusID])");
            }
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

        private static void TruncateTables()
        {
            using (var c = Util.GetOpenConnection())
            {
                try
                {
                    c.Execute("ALTER TABLE [dbo].[Book] DROP CONSTRAINT [FK_Book_Author]");
                    c.Execute("ALTER TABLE [dbo].[Author] DROP CONSTRAINT [FK_Author_AuthorStatus]");
                }
                catch { }

                c.Execute("TRUNCATE TABLE Author;");
                c.Execute("TRUNCATE TABLE Book;");
                c.Execute("TRUNCATE TABLE AuthorStatus;");
            }
        }

        DataTable BuildDataTableBooks(int from, int count, List<string> words)
        {
            var dt = new DataTable();
            dt.Columns.Add("BookID", typeof(int));
            dt.Columns.Add("AuthorID", typeof(int));
            dt.Columns.Add("Title", typeof(string));

            int counter = 1;
            for (int i = from; i < from + count; i++)
            {
                List<Book> books = MakeRandomNumberOfBooks(words);
                foreach (var book in books)
                {
                    DataRow row = dt.NewRow();
                    row.ItemArray = new object[] { counter, i + 1, book.Title };
                    dt.Rows.Add(row);
                    counter++;
                }
            }
            return dt;
        }

        public DataTable BuildDataTableAuthors(int from, int count, List<string> firstnames, List<string> surnames, List<string> words)
        {
            var dt = new DataTable();
            dt.Columns.Add("AuthorID", typeof(int));
            dt.Columns.Add("AuthorStatusID", typeof(int));
            dt.Columns.Add("FirstName", typeof(string));
            dt.Columns.Add("LastName", typeof(string));
            dt.Columns.Add("DateOfBirth", typeof(DateTime));
            //dt.Columns.Add("EmailAddress", typeof(string));

            var r = GetRandom();
            for (int i = from; i < from + count; i++)
            {
                Author author = MakeRandomAuthor(firstnames, surnames);
                DataRow row = dt.NewRow();
                // this looks like AuthorID initially will be 0, however it is 1 (identity).. so its just a key for the dt?
                //row.ItemArray = new object[] { i, author.FirstName, author.LastName, author.EmailAddress };
                row.ItemArray = new object[] { i, r.Next(1, 3), author.FirstName, author.LastName, author.DateOfBirth };
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
        static Random rnd;
        static Random GetRandom()
        {
            if (rnd != null) return rnd;
            rnd = new Random();
            return rnd;
        }

        public List<Book> MakeRandomNumberOfBooks(List<string> words)
        {
            var books = new List<Book>();
            for (int j = 0; j < GetRandom().Next(1, 5); j++)
            {
                Book book = GetBookTitle(words);
                books.Add(book);
            }
            return books;
        }

        public Book GetBookTitle(List<string> words)
        {
            var r = GetRandom();
            int firstWordIndex = r.Next(words.Count);
            string firstWord = words[firstWordIndex].CapitaliseFirstLetter();

            // unless want to run out of words!
            //words.RemoveAt(firstWordIndex);

            int secondWordIndex = r.Next(words.Count);
            string secondWord = words[secondWordIndex].CapitaliseFirstLetter();

            var book = new Book { Title = firstWord + " of the " + secondWord };
            return book;
        }

        // Stored Proc
        //public AuthorsViewModel GetAuthors2(string sortColumnAndDirection, int? authorIDFilter,
        //    string firstNameFilter, string lastNameFilter, DateTime? dateOfBirthFilter, int numberOfResults)
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

        //        p.Add("@AuthorID", authorIDFilter);
        //        if (firstNameFilter == "")
        //            p.Add("@FirstName");
        //        else
        //            p.Add("@FirstName", firstNameFilter);

        //        if (lastNameFilter == "")
        //            p.Add("@LastName");
        //        else
        //            p.Add("@LastName", lastNameFilter);

        //        p.Add("@DateOfBirth", dateOfBirthFilter);
        //        p.Add("@NumberOfResults", numberOfResults);
        //        p.Add("@PageNumber", 2);

        //        // keep the main query, and count logic in 1 SP which returns 2 record sets (the results, and a single row - count)
        //        var x = db.QueryMultiple("GetAuthors", p, commandType: CommandType.StoredProcedure);
        //        var result = x.Read<Author>().ToList();
        //        var count = x.Read<int>().Single();
        //        var vm = new AuthorsViewModel
        //        {
        //            Authors = result,
        //            CountOfAuthors = count
        //        };
        //        return vm;
        //    }
        //}

    }
}