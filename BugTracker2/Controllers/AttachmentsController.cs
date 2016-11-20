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
            var attachments = db.Attachments.Include(a => a.User).Include(a => a.Tickets);
            return View(attachments.ToList());
        }

        // GET: Attachments/Details/5
        public ActionResult Details(int? id)
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

        // GET: Attachments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Attachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "TicketsId")] Attachments attachment, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                attachment.Created = DateTimeOffset.Now;
                attachment.SubmitterId = User.Identity.GetUserId();

            if (image != null && image.ContentLength > 0)
            {
                    //relative server path
                    var filePath = "/fileUpload/";
                    //path on physical drive on server
                    var absPath = Server.MapPath("~" + filePath);
                    // media url for relative path
                    attachment.MediaUrl = filePath + image.FileName;
                    //save image
                    image.SaveAs(Path.Combine(absPath, image.FileName));

                    //check the file name to make sure its an image
                    var ext = Path.GetExtension(image.FileName).ToLower();
                    if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                    ModelState.AddModelError("image", "Invalid Format.");

                    db.Attachments.Add(attachment);
                    db.SaveChanges();
                    return RedirectToAction("Details", "Tickets", new { id = attachment.TicketsId });
                    }
                  }
                    return View(attachment);
              }

     // GET: Attachments/Edit/5
        public ActionResult Edit(int? Id)
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
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", attachments.SubmitterId);
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
            ViewBag.OwnerId = new SelectList(db.Users, "Id", "FirstName", attachments.SubmitterId);
            ViewBag.TicketsId = new SelectList(db.Tickets, "Id", "OwnerId", attachments.TicketsId);
            return View(attachments);
        }

        // GET: Attachments/Delete/5
        public ActionResult Delete(int? Id)
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

        // POST: Attachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int Id)
        {
            Attachments attachments = db.Attachments.Find(Id);
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

