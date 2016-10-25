using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThinkBooksWebsite.Services;

namespace ThinkBooksWebsite.Controllers
{
    public class BooksController : Controller
    {
        BooksRepository db = new BooksRepository();

        // GET: Books
        public ActionResult Index()
        {
            BooksViewModel vm = db.GetBooks();
            var books = vm.Books;
            return View(books);
        }
    }
}