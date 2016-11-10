using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using ThinkBooksWebsiteTesting.Models;

namespace ThinkBooksWebsiteTesting.Services
{
    //public class AuthorsViewModel
    //{
    //    public List<Author> Authors { get; set; }
    //    public int CountOfAuthors { get; set; }
    //}

    public class BookStatusRepository
    {
        public IList<BookStatus> GetBookStatuses()
        {
            using (var db = Util.GetOpenConnection())
            {
                return db.GetAll<BookStatus>().ToList();
            }
        }

        public BookStatus GetBookStatus(int i)
        {
            using (var db = Util.GetOpenConnection())
            {
                return db.Get<BookStatus>(i);
            }
        }

        public void InsertBookStatus(BookStatus bookStatus)
        {
            using (var db = Util.GetOpenConnection())
            {
                db.Insert(bookStatus);
            }
        }

        public void UpdateBookStatus(BookStatus bookStatus)
        {
            using (var db = Util.GetOpenConnection())
            {
                db.Update(bookStatus);
            }
        }

        public void DeleteBookStatus(int id)
        {
            using (var db = Util.GetOpenConnection())
            {
                db.Delete(new BookStatus {BookStatusID = id});
            }
        }
    }
}