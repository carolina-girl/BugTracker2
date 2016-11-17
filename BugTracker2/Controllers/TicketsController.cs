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

            if (User.IsInRole("ProjectManager"))
            {
                var mytickets = (from b in db.Tickets
                                 where b.OwnerId == userId
                                 select b).ToList();


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


        // GET: Tickets/FullList/5
        [Authorize(Roles = "Admin")]
        public ActionResult FullList()
        {
            List<Tickets> tickets = new List<Tickets>();
            if (User.IsInRole("Admin"))
            {
                tickets = db.Tickets.ToList();
            }

            return View(tickets);
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
            tickets.Updated = DateTimeOffset.Now;
            db.SaveChanges();
            return View();
        }

        // GET: Tickets/Create
        //[Authorize(Roles = "Submitter,Admin")]
        public ActionResult Create(int? id)
        {
            ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status");

            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority");

            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type");

            ViewBag.CreatedUserId = new SelectList(db.Users, "Id", "DisplayName");

            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            //ViewBag.ProjectId = new SelectList(user.Projects, "Id", "Title");
            return View();
        }

        //POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Body,ProjectId")] Tickets ticket)
        {
            if (ModelState.IsValid)
            {

                ticket.Created = DateTimeOffset.Now;
                List<Projects> Project = new List<Projects>();
                var ticketId = db.Projects.FirstOrDefault(p => p.Id == ticket.Id);

                List<Tickets> Ticket = new List<Tickets>();

                var ticketPriority = db.TicketPriorities.FirstOrDefault(t => t.Id == ticket.Id);
                ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority", ticket.Priority);

                var ticketType = db.TicketTypes.FirstOrDefault(t => t.Id == ticket.Id);
                ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type", ticket.Type);

                ticket.Status = db.TicketStatus.FirstOrDefault(s => s.Status == "Open");
                ticket.OwnerId = User.Identity.GetUserId();
                db.Tickets.Add(ticket);
             
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View();
        }

        // GET: Tickets/Edit/5
        [Authorize(Roles = "Admin, ProjectManager, Submitter, Developer")]
        public ActionResult Edit(int? id)
        {
            Tickets ticket = db.Tickets.Find(id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            var devId = db.Roles.FirstOrDefault(d => string.Compare("Developer", d.Name, true) == 0).Id;
            var developers = db.Users.Where(r => r.Roles.Any(a => a.RoleId == devId));
            ViewBag.AssignedUserId = new SelectList(developers,"Id", "FirstName", ticket.AssignedUserId);

            ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status", ticket.Status);
            
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority", ticket.Priority);

            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type", ticket.Type);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OwnerId,Created,Updated,Title,Body,StatusId,AssignedUserId,StatusId,PriorityId,TypeId,ProjectId")] Tickets ticket)
        {
            var devId = db.Roles.FirstOrDefault(d => string.Compare("Developer", d.Name, true) == 0).Id;
            var developers = db.Users.Where(r => r.Roles.Any(a => a.RoleId == devId));

            if (ModelState.IsValid)
            {
                ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status", "tickets.Status");
                List<Tickets> projects = new List<Tickets>();
                if (User.IsInRole("Admin"))
                {
                    var UserId = User.Identity.GetUserId();
                    ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority", "tickets.Priority");
                } 
                    
                if (User.IsInRole("ProjectManager") || User.IsInRole("Developer") || User.IsInRole("Submitter"))
                {
                    var UserId = User.Identity.GetUserId();
                    ViewBag.PriorityId = new SelectList(db.TicketPriorities.Where(p => p.Priority == UserId)).ToList();   
                }
                ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type", "tickets.Type");

                ticket.Updated = DateTimeOffset.Now;
                List<Projects> Project = new List<Projects>();
                var ticketId = db.Projects.FirstOrDefault(p => p.Id == ticket.Id);
                db.Tickets.Attach(ticket);
                //tickets.OwnerId = User.Identity.GetUserId();
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status", ticket.Status);
            ViewBag.AssignedUserId = new SelectList(developers, "Id", "DisplayName", ticket.AssignedUserId);
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority", ticket.Priority);
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type", ticket.Type);
            return View(ticket);
           }

        //GET: Tickets/History/5
        public ActionResult History()
        {
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
        
    



