using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ThinkBooksWebsiteValidationTest.Models;

namespace ThinkBooksWebsiteValidationTest.Models
{
    //http://stackoverflow.com/questions/16736494/add-data-annotations-to-a-class-generated-by-entity-framework
    [MetadataType(typeof(AuthorMetaData))]
    public partial class Author
    {
    }

    public class AuthorMetaData
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        //public Nullable<System.DateTime> DateOfBirth { get; set; }
        //public virtual AuthorStatu AuthorStatu { get; set; }
    }
}

namespace ThinkBooksWebsiteValidationTest.Controllers
{
    public class AuthorsController : Controller
    {
        private ThinkBooksMonday db = new ThinkBooksMonday();

        // GET: Authors
        public ActionResult Index()
        {
            var authors = db.Authors.Include(a => a.AuthorStatu).Take(10);
            return View(authors.ToList());
        }

        // GET: Authors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Authors.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // GET: Authors/Create
        public ActionResult Create()
        {
            ViewBag.AuthorStatusID = new SelectList(db.AuthorStatus, "AuthorStatusID", "Name");
            return View();
        }

        // POST: Authors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AuthorID,AuthorStatusID,FirstName,LastName,DateOfBirth")] Author author)
        {
            if (ModelState.IsValid)
            {
                db.Authors.Add(author);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AuthorStatusID = new SelectList(db.AuthorStatus, "AuthorStatusID", "Name", author.AuthorStatusID);
            return View(author);
        }

        // GET: Authors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Authors.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            ViewBag.AuthorStatusID = new SelectList(db.AuthorStatus, "AuthorStatusID", "Name", author.AuthorStatusID);
            return View(author);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AuthorID,AuthorStatusID,FirstName,LastName,DateOfBirth")] Author author)
        {
            if (ModelState.IsValid)
            {
                db.Entry(author).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AuthorStatusID = new SelectList(db.AuthorStatus, "AuthorStatusID", "Name", author.AuthorStatusID);
            return View(author);
        }

        // GET: Authors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Author author = db.Authors.Find(id);
            if (author == null)
            {
                return HttpNotFound();
            }
            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Author author = db.Authors.Find(id);
            db.Authors.Remove(author);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
