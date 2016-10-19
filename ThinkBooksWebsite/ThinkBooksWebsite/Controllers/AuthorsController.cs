using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThinkBooksWebsite.Services;

namespace ThinkBooksWebsite.Controllers
{
    public class Author
    {
        public int AuthorID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class AuthorsController : Controller
    {
        // GET: Authors - the default view too ie /
        public ActionResult Index()
        {
            var db = new AuthorRepository();
            List<Author> authors = db.GetAuthors();
            return View(authors);
        }
    }
}