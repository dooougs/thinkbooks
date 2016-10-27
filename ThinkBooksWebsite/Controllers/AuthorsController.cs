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

        // using buttons - when they are not pressed, they don't send
        // sortColumnAndDirection
        public ActionResult Index(string sortColumnAndDirection = "AuthorID", string currentSortOrder = "AuthorID", int page = 1, int currentPage = 1, int? authorIDFilter = null, string firstNameFilter = null, 
            string lastNameFilter = null, DateTime? dateOfBirthFilter = null, string results = "50")
        {
            // clicked on next or previous button, so keep the sortOrder. 
            // unless page == 1, which means a sort button has been pressed
            if (page != currentPage && page != 1)
            {
                sortColumnAndDirection = currentSortOrder;
            } 

            // have pressed a sort column, want to reset to page 1
            if (sortColumnAndDirection != currentSortOrder)
            {
                page = 1;
            }

            ViewBag.CurrentSortOrder = sortColumnAndDirection;
            ViewBag.CurrentPage = page;

            int numberOfResults;
            if (!int.TryParse(results, out numberOfResults))
            {
                // All selected so limit to 1m
                numberOfResults = 1000000;
            }
            var vm = db.GetAuthors(sortColumnAndDirection, authorIDFilter, firstNameFilter, lastNameFilter,
                dateOfBirthFilter, numberOfResults, page);

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

            // paging
            var records = vm.CountOfAuthors;
            var recordsPerPage =  int.Parse(results);
            int pageCount = (records + recordsPerPage - 1) / recordsPerPage;
            ViewBag.PageCount = pageCount;
            ViewBag.NextPage = page == pageCount ? pageCount : page + 1;
            ViewBag.PreviousPage = page == 1 ? 1 : page - 1;

            return View(authors);
        }

        public ActionResult LoadDataSqlBulkCopyAuthorsBooks()
        {
            db.LoadDataSqlBulkCopyAuthorsBooks();
            return RedirectToAction("index");
        }
    }
}