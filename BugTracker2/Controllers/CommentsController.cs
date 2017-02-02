using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker2.Models;
using Microsoft.AspNet.Identity;
using System.Text;
using System.Xml.Linq;

namespace BugTracker2.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            var comment = db.Comments.Include(c => c.User);
            return View(comment.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comments comment = db.Comments.Find(Id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        //// GET: Comments/Create
        //public ActionResult Create()
        //{
        //    ViewBag.TicketsId = new SelectList(db.Tickets, "Id", "Title");
        //    ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName");
        //    return View();
        //}

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //public ActionResult Create([Bind(Include = "Id,Body,TicketsId")] Comments comment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //                comment.Created = DateTimeOffset.Now;
        //                comment.Updated = DateTimeOffset.Now;
        //                comment.UserId = User.Identity.GetUserId();
        //                var com = db.Tickets.FirstOrDefault(p => p.Id == comment.TicketsId).Id;
        //                db.Comments.Add(comment);
        //                comment = db.Comments.Find(comment.Id);
        //                db.SaveChanges();
        //TicketHistory history = new TicketHistory();
        //history.Date = DateTimeOffset.Now;
        //var historyBody = comment.UserId + "has added a comment to this ticket.";
        //history.Body = historyBody;
        //history.TicketId = comment.TicketsId;
        //db.TicketHistory.Add(history);

        //return RedirectToAction("Details", "Tickets", new { id = comment.Tickets.Id });
        //    }
        //    return View(comment);
        //}

        //GET: Comments/Create
        [Authorize]
        public ActionResult Create(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            var userId = User.Identity.GetUserId();
            ViewBag.TicketsId = id;
            ViewBag.Title = ticket.Title;
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketsId,Body")] Comments comment)
        {
            if (ModelState.IsValid)
            {
                comment.UserId = User.Identity.GetUserId();
                comment.Created = System.DateTimeOffset.Now;
                db.Comments.Add(comment);
                db.SaveChanges();

                var ticket = db.Tickets.Find(comment.TicketsId);
                var assigneeId = ticket.AssignedUserId;

                return RedirectToAction("Details", "Tickets", new { id = comment.TicketsId });
            }
            return View();
        }



        // GET: Comments/Edit/5
        public ActionResult Edit(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comments comment = db.Comments.Find(Id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", comment.UserId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketsId,UserId,Body,Submitted,UpdatedReason,Created,Updated")] Comments comment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(comment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.Users, "Id", "FirstName", comment.UserId);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comments comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comments comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = comment.TicketsId });
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
