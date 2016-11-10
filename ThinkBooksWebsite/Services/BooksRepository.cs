using Dapper;
using System.Collections.Generic;
using System.Linq;
using ThinkBooksWebsite.Models;

namespace ThinkBooksWebsite.Services
{
    public class BooksViewModel
    {
        public List<Book> Books { get; set; }
        public int CountOfBooks { get; set; }
    }

    public class BooksRepository
    {
        public BooksViewModel GetBooks()
        {
            using (var db = Util.GetOpenConnection())
            {
                var result = db.Query<Book>("SELECT TOP 10 * FROM Book").ToList();

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