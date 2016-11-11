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
using System.Web.Security;
using Microsoft.AspNet.Identity.Owin;

namespace BugTracker2.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //GET: Tickets
        public ActionResult Index()
        {
            List<Tickets> tickets = new List<Tickets>();
            var userId = User.Identity.GetUserId();
            if (User.IsInRole("Admin"))
            {
                tickets = db.Tickets.ToList();
            }

            if (User.IsInRole("Project Manager"))
            {
                var mytickets = (from p in db.Projects
                                 where p.Users.Any(r => r.Id == userId)
                                 select p).ToList();

                foreach (var p in mytickets)
                {
                    foreach (var t in db.Tickets)
                    {
                        if (p.Id == t.ProjectId)
                        {
                            mytickets.Add(p);
                        }
                    }
                }
                return View(mytickets);
            }
            if (User.IsInRole("Submitter"))
            {
                var mytickets = (from b in db.Tickets
                                 where b.OwnerId == userId
                                 select b).ToList();
                return View(mytickets);
            }

            if (User.IsInRole("Developer"))
            {
                var mytickets = (from b in db.Tickets
                                 where b.AssignedUserId == userId
                                 select b).ToList();
                return View(mytickets);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // GET: Tickets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            return View(tickets);
        }

        // POST: Tickets/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details([Bind(Include = "Id,Title,OwnerId,Body,Created,Updated,ProjectId")] Tickets tickets)
        {
            List<Tickets> Ticket = new List<Tickets>();
            var assignedUser = tickets.AssignedUserId;
            ViewBag.AssignedUserId = new SelectList(db.Users, "Developer", "FirstName");
            //TicketStatus status = db.TicketStatus.FirstOrDefault(s => s.Status == "Pending");
            var pending = db.TicketStatus.FirstOrDefault(s => s.Status == "Pending");
            if (tickets.StatusId == pending.Id && tickets.AssignedUserId != null)
                tickets.StatusId = db.TicketStatus.FirstOrDefault(t => t.Status == "Pending").Id;

            //var resolvedStatus = db.TicketStatus.FirstOrDefault(s => s.Status == "Closed");
            //if (tickets.StatusId == resolvedStatus.Id)

            tickets.Updated = DateTimeOffset.Now;
            db.SaveChanges();
            return View();
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter, Admin")]
        public ActionResult Create()
        {
            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "FirstName");
            ViewBag.CreatedUserId = new SelectList(db.Users, "Id", "FirstName");
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            ViewBag.ProjectId = new SelectList(user.Projects, "Id", "Title");
            return View();
        }

        //POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,OwnerId,Body,ProjectId")] Tickets tickets)
        {
            if (ModelState.IsValid)
            {
                tickets.Created = DateTimeOffset.Now;
                List<Tickets> Ticket = new List<Tickets>();
                tickets.OwnerId = User.Identity.GetUserId();
                db.Tickets.Add(tickets);
                tickets.Status = db.TicketStatus.FirstOrDefault(s => s.Status == "Open");
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tickets);
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Submitter, Admin")]
        public ActionResult Edit(int? id)
        {
            ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status", "tickets.Status");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            return View(tickets);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OwnerId,CreatedDate,UpdatedDate,Title,Body,AssignedUser,StatusId")] Tickets tickets)
        {
            if (ModelState.IsValid)
            {

                //var pending = db.TicketStatus.FirstOrDefault(s => s.Status == "Pending");
                //if (tickets.StatusId== pending.Id && tickets.AssignedUserId != null)
                //    tickets.StatusId = db.TicketStatus.FirstOrDefault(t => t.Status == "Pending").Id;

                //var resolvedStatus = db.TicketStatus.FirstOrDefault(s => s.Status == "Closed");
                //if (tickets.StatusId == resolvedStatus.Id)

                tickets.Updated = DateTimeOffset.Now;
                db.Entry(tickets).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
             }
                ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status", "tickets.Status");
                return RedirectToAction("Index");
             }
        



        // GET: Tickets/CommentsAttachments/5
        public ActionResult CommentsAttachments(int? id)
        {
            var userId = User.Identity.GetUserId();
        var tickets = new List<Tickets>();
        var attachments = new List<Attachments>();
        var comments = new List<Comments>();
        int projects = 0;
          

            if (User.IsInRole("Administrator"))
            {
                tickets = db.Tickets.ToList();
                attachments = db.Attachments.ToList();
                foreach (var ticket in tickets)
                    foreach (var comment in ticket.Comments)
                        comments.Add(comment);
            }
            else if (User.IsInRole("Project Manager"))
            {
                tickets = db.Tickets.Where(t => t.OwnerId == userId).ToList();
                foreach (var ticket in tickets)
                    foreach (var attach in ticket.Attachments)
                        attachments.Add(attach);
                foreach (var ticket in tickets)
                    foreach (var comment in ticket.Comments)
                        comments.Add(comment);
                projects = userId.ToList().Count();
}
            else if (User.IsInRole("Developer") && User.IsInRole("Project Manager"))
            {
                tickets = db.Tickets.Where(t => t.Project.Users.Contains(db.Users.Find(userId))).ToList();
                foreach (var ticket in tickets)
                    foreach (var attach in ticket.Attachments)
                        attachments.Add(attach);
                foreach (var ticket in tickets)
                    foreach (var comment in ticket.Comments)
                        comments.Add(comment);
                projects = userId.ToList().Count();
            }
            else if (User.IsInRole("Developer"))
            {
                tickets = db.Tickets.Where(t => t.AssignedUserId == userId).ToList();
                foreach (var ticket in tickets)
                    foreach (var attach in ticket.Attachments)
                         attachments.Add(attach);
                foreach (var ticket in tickets)
                    foreach (var comment in ticket.Comments)
                        comments.Add(comment);
                projects = userId.ToList().Count();
            }

            var model = new DashboardViewModel()
            {
                Tickets = tickets,
                Attachments = attachments,
                Comments = comments.Take(5),
                ProjectsAmt = projects
            };

            return View();
        }

        //GET: Tickets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            return View(tickets);
        }

        //POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tickets tickets = db.Tickets.Find(id);
            db.Tickets.Remove(tickets);
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


