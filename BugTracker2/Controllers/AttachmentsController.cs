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
using System.IO;

namespace BugTracker2.Controllers
{
    public class AttachmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Attachments
        public ActionResult Index()
        {
            var attachments = db.Attachments.Include(a => a.Owner).Include(a => a.Tickets);
            return View(attachments.ToList());
        }


        // GET: Attachments
        public new ActionResult View()
        {
            var attachments = db.Attachments.Include(a => a.Owner).Include(a => a.Tickets);
            return View(attachments.ToList());
        }



        // GET: Attachments/Details/5
        public ActionResult Details(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attachments attachments = db.Attachments.Find(Id);
            if (attachments == null)
            {
                return HttpNotFound();
            }
            return View(attachments);
        }

        // GET: Attachments/Create
        public ActionResult Create(string Id)
        {
            var tickets = db.Tickets.Find(Id);
            var attachments = new Attachments();
            attachments.Tickets = tickets;
            //attachments.TicketsId = tickets.Id;

            return View(attachments);
        }

        // POST: Attachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketsId,Body,SubmitterId,Title,MediaUrl,Submitted")] Attachments attachments, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                attachments.Created = DateTimeOffset.Now;
                attachments.SubmitterId = User.Identity.GetUserId();

                db.Attachments.Add(attachments);
                db.SaveChanges();

                return RedirectToAction("Details", "Tickets", new { Id = attachments.SubmitterId });
            }
            return RedirectToAction("Details", "Tickets", new { Id = attachments.SubmitterId });
        }


        // GET: Attachments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attachments attachments = db.Attachments.Find(id);
            if (attachments == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerId = new SelectList(db.ApplicationUsers, "Id", "FirstName", attachments.SubmitterId);
            ViewBag.TicketsId = new SelectList(db.Tickets, "Id", "OwnerId", attachments.TicketsId);
            return View(attachments);
        }

        // POST: Attachments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketsId,SubmitterId,Body,MediaUrl,OwnerId,Created")] Attachments attachments)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attachments).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerId = new SelectList(db.ApplicationUsers, "Id", "FirstName", attachments.SubmitterId);
            ViewBag.TicketsId = new SelectList(db.Tickets, "Id", "OwnerId", attachments.TicketsId);
            return View(attachments);
        }

        // GET: Attachments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attachments attachments = db.Attachments.Find(id);
            if (attachments == null)
            {
                return HttpNotFound();
            }
            return View(attachments);
        }

        // POST: Attachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Attachments attachments = db.Attachments.Find(id);
            db.Attachments.Remove(attachments);
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

