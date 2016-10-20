using System.Collections.Generic;
using System.Web.Mvc;
using ThinkBooksWebsite.Models;
using ThinkBooksWebsite.Services;

namespace ThinkBooksWebsite.Controllers
{
    public class AuthorsController : Controller
    {
        // GET: Authors - the default view too ie /
        public ActionResult Index(string sortOrder = "LastName", string sortDirection = "ASC")
        {
            var db = new AuthorsRepository();
            List<Author> authors = db.GetAuthors(sortOrder, sortDirection);
            return View(authors);
        }
    }
}