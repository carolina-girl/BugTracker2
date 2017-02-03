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
            var attachment = db.Attachments.Include(a => a.User).Include(a => a.TicketsId);
            return View(attachment.ToList());
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

        //// GET: Attachments/Create
        //public ActionResult Create(int id)
        //{
        //    Attachments attachment = db.Attachments.Find(id);
        //    db.Attachments.Remove(attachment);
        //    db.SaveChanges();
        //    return RedirectToAction("Details", "Tickets", new { id = attachment.TicketsId });
        //}

        //// POST: Attachments/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //public ActionResult Create([Bind(Include = "TicketsId")] Attachments attachment, HttpPostedFileBase image)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        attachment.Created = DateTimeOffset.Now;
        //        attachment.SubmitterId = User.Identity.GetUserId();

        //        if (image != null && image.ContentLength > 0)
        //        {
        //            //relative server path
        //            var filePath = "/fileUpload/";
        //            //path on physical drive on server
        //            var absPath = Server.MapPath("~" + filePath);
        //            // media url for relative path
        //            attachment.MediaUrl = filePath + image.FileName;
        //            //save image
        //            image.SaveAs(Path.Combine(absPath, image.FileName));

        //            //check the file name to make sure its an image
        //            var ext = Path.GetExtension(image.FileName).ToLower();
        //            //if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
        //            //ModelState.AddModelError("image", "Invalid Format.");

        //            attachment.UserId = User.Identity.GetUserId();
        //            var com = db.Tickets.FirstOrDefault(p => p.Id == attachment.TicketsId).Id;
        //            db.Attachments.Add(attachment);
        //            db.SaveChanges();
        //            return RedirectToAction("Details", "Tickets", new { id = attachment.TicketsId });
        //        }
        //    }
        //    ViewBag.TicketsId = new SelectList(db.Tickets, "Id", "OwnerId", attachment.TicketsId);
        //    return View(attachment);
        //}

        //GET: Attachments/Create
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
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TicketsId,Body,MediaUrl")] Attachments attachment, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                attachment.Created = DateTimeOffset.Now;
                attachment.SubmitterId = User.Identity.GetUserId();
                var fileName = Path.GetFileName(image.FileName);
                var uniqueId = DateTime.Now.Ticks;
                fileName = Regex.Replace(fileName, @"[!@#$%_\s]", "");
                image.SaveAs(Path.Combine(Server.MapPath("~/fileUpload/"), uniqueId + fileName));
                attachment.MediaUrl = "/fileUpload/" + uniqueId + fileName;
                
                attachment.UserId = User.Identity.GetUserId();
                db.Attachments.Add(attachment);
                db.SaveChanges();

                var ticket = db.Tickets.Find(attachment.TicketsId);

                return RedirectToAction("Details", "Tickets", new { id = attachment.TicketsId });
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
            ViewBag.TicketsId = new SelectList(db.Tickets, "Id", "OwnerId", attachment.TicketsId);
            return View(attachment);
        }

        // POST: Attachments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TicketsId,SubmitterId,Body,MediaUrl,OwnerId,Created")] Attachments attachment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(attachment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", attachment.SubmitterId);
            ViewBag.TicketsId = new SelectList(db.Tickets, "Id", "OwnerId", attachment.TicketsId);
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
            return RedirectToAction("Details", "Tickets", new { id = attachment.TicketsId });
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

