using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ThinkBooksWebsiteTesting.Models;
using ThinkBooksWebsiteTesting.Services;

namespace ThinkBooksWebsiteTesting.Controllers
{
    public class BookStatusController : Controller
    {
        BookStatusRepository db = new BookStatusRepository();

        public ActionResult Index()
        {
            return View(db.GetBookStatuses());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var bookStatus = db.GetBookStatus((int)id);
            if (bookStatus == null)
            {
                return HttpNotFound();
            }
            return View(bookStatus);
        }

        public ActionResult Create()
        {
            return View();
        }

        // POST: BookStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BookStatusID,Name")] BookStatus bookStatus)
        {
            if (ModelState.IsValid)
            {
                db.InsertBookStatus(bookStatus);
                return RedirectToAction("Index");
            }

            return View(bookStatus);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var bookStatus = db.GetBookStatus((int)id);
            if (bookStatus == null)
            {
                return HttpNotFound();
            }
            return View(bookStatus);
        }

        // POST: BookStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookStatusID,Name")] BookStatus bookStatus)
        {
            if (ModelState.IsValid)
            {
                db.UpdateBookStatus(bookStatus);
                return RedirectToAction("Index");
            }
            return View(bookStatus);
        }

        // GET: BookStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var bookStatus = db.GetBookStatus((int)id);
            if (bookStatus == null)
            {
                return HttpNotFound();
            }
            return View(bookStatus);
        }

        // POST: BookStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //BookStatus bookStatus = db.DeleteBookStatus((int)id);
            db.DeleteBookStatus(id);
            return RedirectToAction("Index");
        }
    }
}
