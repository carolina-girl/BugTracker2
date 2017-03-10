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
using System.Text.RegularExpressions;

namespace BugTracker2.Controllers
{
    public class AttachmentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        // GET: Attachments
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            var ticketsId = ViewBag.TicketsId;
            var attachments = new List<Attachments>();
            attachments = user.Attachments.ToList();
            return View(attachments);
        }


    // GET: Attachments/Details/5
    public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attachments attachment = db.Attachments.Find(id);
            if (attachment == null)
            {
                return HttpNotFound();
            }
            return View(attachment);
        }

        //GET: Attachments/Create
        [Authorize]
        public ActionResult Create(int? id, int? TicketsId)
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
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketsId,Body,MediaUrl,")] Attachments attachment, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (UploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    var uniqueId = DateTime.Now.Ticks;
                    fileName = Regex.Replace(fileName, @"[!@#$%_\s]", "");
                    image.SaveAs(Path.Combine(Server.MapPath("~/fileUpload/"), uniqueId + fileName));
                    attachment.MediaUrl = "/fileUpload/" + uniqueId + fileName;
                    }
                    else
                    {
                        return View();
                    }
                attachment.Created = DateTimeOffset.Now;
                attachment.UserId = User.Identity.GetUserId();
                db.Attachments.Add(attachment);
                db.SaveChanges();

                    TicketHistory history = new TicketHistory();
                    history.Date = DateTimeOffset.Now;
                    var historyBody = "A new attachment was added to this ticket.";
                    history.Body = historyBody;
                    history.TicketId = attachment.ticketsId;
                    db.TicketHistory.Add(history);
                    db.SaveChanges();

                return RedirectToAction("Details", "Tickets", new { id = attachment.ticketsId });
  
                }
               return View();
             }


        // GET: Attachments/Edit/5
        public ActionResult Edit(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attachments attachment = db.Attachments.Find(Id);
            if (attachment == null)
            {
                return HttpNotFound();
            }
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", attachment.SubmitterId);
            ViewBag.TicketsId = new SelectList(db.Tickets, "Id", "OwnerId", attachment.Tickets);
            return View(attachment);
        }

        // POST: Attachments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketsId,SubmitterId,Body,MediaUrl,OwnerId,Created,Updated")] Attachments attachment, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                attachment = db.Attachments.Find(image);
                var fileName = Path.GetFileName(image.FileName);
                var uniqueId = DateTime.Now.Ticks;
                fileName = Regex.Replace(fileName, @"[!@#$%_\s]", "");
                image.SaveAs(Path.Combine(Server.MapPath("~/fileUpload/"), uniqueId + fileName));
                attachment.MediaUrl = "/fileUpload/" + uniqueId + fileName;
                var ticket = db.Tickets.Find(attachment.Tickets);
                attachment.UserId = User.Identity.GetUserId();
                ticket = db.Tickets.Find(attachment.Tickets);
                var assignedUserId = ticket.AssignedUserId;
                db.Entry(attachment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", attachment.SubmitterId);
            ViewBag.TicketsId = new SelectList(db.Tickets, "Id", "OwnerId", attachment.Tickets);
            return View(attachment);
        }



        // GET: Attachments/Delete/5
        public ActionResult Delete(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Attachments attachment = db.Attachments.Find(Id);
            if (attachment == null)
            {
                return HttpNotFound();
            }
            return View(attachment);
        }

        // POST: Attachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int Id)
        {
            Attachments attachment = db.Attachments.Find(Id);
            db.Attachments.Remove(attachment);
            db.SaveChanges();
            return RedirectToAction("Details", "Tickets", new { id = attachment.Tickets });
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

