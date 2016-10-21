using System.Collections.Generic;
using System.Web.Mvc;
using ThinkBooksWebsite.Models;
using ThinkBooksWebsite.Services;

namespace ThinkBooksWebsite.Controllers
{
    public class AuthorsController : Controller
    {
        public ActionResult Index(string s = "LastName")
        {
            var sortColumnAndDirection = s;
            var db = new AuthorsRepository();
            List<Author> authors = db.GetAuthors(sortColumnAndDirection);

            // Flip the order of the sort param on the button so next time it is pressed with reverse current
            ViewBag.SortParamAuthorID = sortColumnAndDirection == "AuthorID" ? "AuthorID_desc" : "AuthorID";
            ViewBag.SortParamFirstName = sortColumnAndDirection == "FirstName" ? "FirstName_desc" : "FirstName";
            ViewBag.SortParamLastName = sortColumnAndDirection == "LastName" ? "LastName_desc" : "LastName";
            ViewBag.SortParamDateOfBirth = sortColumnAndDirection == "DateOfBirth" ? "DateOfBirth_desc" : "DateOfBirth";
            return View(authors);
        }
    }
}