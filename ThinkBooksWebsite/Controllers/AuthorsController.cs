using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ThinkBooksWebsite.Models;
using ThinkBooksWebsite.Services;

namespace ThinkBooksWebsite.Controllers
{
    public class AuthorsController : Controller
    {
        AuthorsRepository db = new AuthorsRepository();

        public ActionResult Index(string results = "50", string s = "LastName", int? authorIDFilter = null, string firstNameFilter = null, 
            string lastNameFilter = null, DateTime? dateOfBirthFilter = null )
        {
            var sortColumnAndDirection = s;

            int numberOfResults;
            if (!int.TryParse(results, out numberOfResults))
            {
                // All selected so limit to 1m
                numberOfResults = 1000000;
            }
            var vm = db.GetAuthors(sortColumnAndDirection, authorIDFilter, firstNameFilter, lastNameFilter,
                dateOfBirthFilter, numberOfResults);

            List<Author> authors = vm.Authors;
            ViewBag.TotalQueryCountOfAuthors = vm.CountOfAuthors;

            // Flip the order of the sort param on the button so next time it is pressed with reverse current
            ViewBag.SortParamAuthorID = sortColumnAndDirection == "AuthorID" ? "AuthorID_desc" : "AuthorID";
            ViewBag.SortParamFirstName = sortColumnAndDirection == "FirstName" ? "FirstName_desc" : "FirstName";
            ViewBag.SortParamLastName = sortColumnAndDirection == "LastName" ? "LastName_desc" : "LastName";
            ViewBag.SortParamDateOfBirth = sortColumnAndDirection == "DateOfBirth" ? "DateOfBirth_desc" : "DateOfBirth";

            // Keep filters sticky
            ViewBag.AuthorIDFilter = authorIDFilter;
            ViewBag.FirstNameFilter = firstNameFilter;
            ViewBag.LastNameFilter = lastNameFilter;
            //YYYY - MM - DD.Single digit days and months should be padded with a 0.January is 01.
            if (dateOfBirthFilter != null)
            {
                ViewBag.DateOfBirthFilter = Convert.ToDateTime(dateOfBirthFilter).ToString("yyyy-MM-dd");
            }

            // Keep number of results sticky
            ViewBag.Results = results;

            return View(authors);
        }

        public ActionResult LoadDataSqlBulkCopyAuthorsBooks()
        {
            db.LoadDataSqlBulkCopyAuthorsBooks();
            return RedirectToAction("index");
        }
    }
}