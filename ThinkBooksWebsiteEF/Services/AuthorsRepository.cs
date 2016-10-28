using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Microsoft.Ajax.Utilities;
using StackExchange.Profiling.Helpers.Dapper;
using ThinkBooksWebsiteEF.Models;
using System.Linq.Dynamic;

namespace ThinkBooksWebsiteEF.Services
{
    public class AuthorsRepository
    {
        ThinkBooksEntities db = new ThinkBooksEntities();

        //public AuthorsViewModel GetAuthors(string sortColumnAndDirection, int? authorIDFilter,
        //    string firstNameFilter, string lastNameFilter, DateTime? dateOfBirthFilter, int numberOfResults, int currentPage)
        public AuthorsViewModel GetAuthors(string sortColumnAndDirection)
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

            //var offset = (currentPage - 1) * numberOfResults;

            //if (firstNameFilter == "") firstNameFilter = null;
            //if (lastNameFilter == "") lastNameFilter = null;

            // note % preceding a like search is much slower (250ms to 65ms) and seems a rarer business case to do that
            //var sql = @"
            //        SELECT * FROM Author 
            //        WHERE (@AuthorID IS NULL OR AuthorID = @AuthorID)
            //        AND (@FirstName IS NULL OR FirstName LIKE CONCAT(@FirstName,'%'))
            //        AND (@LastName IS NULL OR LastName LIKE CONCAT(@LastName,'%'))
            //        AND (@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)
            //        ORDER BY " + sanitizedSortColumn + " " + sortDirection + @"
            //        OFFSET " + offset + @" ROWS 
            //        FETCH NEXT " + numberOfResults + " ROWS ONLY";

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

            // System.Linq.Dynamic to be able to sort on column - sanitizedSortColumn
            //var result = db.Authors.Take(20).OrderBy(x => x.AuthorID).ToList();
            var result = db.Authors.Take(20).OrderBy(sortColumn).ToList();
            //var result = db.Authors.Take(20).OrderBy("AuthorID").ToList();



            // seems super fast doing 2 queries ie don't need to do return multiple record sets
            //string sqlCount;
            //if (authorIDFilter == null && dateOfBirthFilter == null && firstNameFilter.IsNullOrWhiteSpace() && lastNameFilter.IsNullOrWhiteSpace())
            //{
            //    sqlCount = @"SELECT SUM(p.rows)
            //              FROM sys.partitions AS p
            //              INNER JOIN sys.tables AS t
            //              ON p.[object_id] = t.[object_id]
            //              INNER JOIN sys.schemas AS s
            //              ON t.[schema_id] = s.[schema_id]
            //              WHERE p.index_id IN (0,1) -- heap or clustered index
            //              AND t.name = N'Author'
            //              AND s.name = N'dbo'";
            //}
            //else
            //{
            //    sqlCount = @"SELECT COUNT(*) FROM Author
            //                    WHERE(@AuthorID IS NULL OR AuthorID = @AuthorID)
            //                    AND(@firstName IS NULL OR FirstName LIKE CONCAT(@firstName, '%'))
            //                    AND (@LastName IS NULL OR LastName LIKE CONCAT(@LastName,'%'))
            //                    AND(@DateOfBirth IS NULL OR DateOfBirth = @DateOfBirth)";
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

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        public void LoadDataSqlBulkCopyAuthorsBooks()
        {
            var numberInBatch = 10000;
            var numberOfBatches = 25;

            TruncateTables();

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

        private static void TruncateTables()
        {
            using (var c = Util.GetOpenConnection())
            {
                try
                {
                    c.Execute("ALTER TABLE [dbo].[Book] DROP CONSTRAINT [FK_Book_Author]");
                }
                catch{}

                c.Execute("TRUNCATE TABLE Author;");
                c.Execute("TRUNCATE TABLE Book;");
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

    }
}