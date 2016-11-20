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
using BugTracker2.Models.Helper;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity.EntityFramework;

namespace BugTracker2.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        private void CreateHist(int newTicketId, string property, string oldValue, string newValue)
        {
            var tHist = new TicketHistory();
            tHist.Id = newTicketId;
            tHist.OldValue = oldValue;
            tHist.NewValue = newValue;
            tHist.Date = DateTimeOffset.UtcNow;
            tHist.HistoryUserId = User.Identity.GetUserId();
            db.Entry(tHist).State = EntityState.Added;
            db.SaveChanges();
            return;
        }
    

//GET: Tickets
public ActionResult Index(string userId)
        {
            userId = User.Identity.GetUserId();
            TicketsHelper helper = new TicketsHelper(db);
            var ticket = helper.GetUserTickets(userId);
            if (User.IsInRole("Admin"))
            {
                ticket = db.Tickets.Where(t => t.AssignedUserId == userId).Include(t => t.AssignedUser).Include(t => t.Owner).Include(t => t.Project).ToList();
            }
            return View(ticket);

        }

        // GET: Tickets/FullList/5
        [Authorize(Roles = "Admin")]
        public ActionResult FullList()
        {
            List<Tickets> ticket = new List<Tickets>();
            if (User.IsInRole("Admin"))
            {
                ticket = db.Tickets.ToList();
            }

            return View(ticket);
        }


        // GET: Tickets/Details/5
        public ActionResult Details(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets ticket = db.Tickets.Find(Id);
            if (ticket == null)
            {
                return HttpNotFound();
            }
            return View(ticket);
        }



        // POST: Tickets/Details/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details([Bind(Include = "Id,Title,OwnerId,Body,Created,Updated,ProjectId")] Tickets ticket)
        {
            List<Tickets> Ticket = new List<Tickets>();
            var assignedUser = ticket.AssignedUserId;
            ViewBag.AssignedUserId = new SelectList(db.Users, "Developer", "FirstName");

            return RedirectToAction("Index");
        }




        // GET: Tickets/Create
        //[Authorize(Roles = "Submitter,Admin")]
        public ActionResult Create(int? Id)
        {
            ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status");

            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority");

            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type");

            ViewBag.CreatedUserId = new SelectList(db.Users, "Id", "DisplayName");
            //V/*iewBag.ProjectId = new SelectList(users.Projects, "Id", "Title");*/

            var userId = User.Identity.GetUserId();
            //var user = db.Users.Find(userId);

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

            return View(ticket);
        }

        // GET: Tickets/Edit/5 
        [Authorize(Roles = "Admin, ProjectManager, Submitter, Developer")]
        public ActionResult Edit(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets ticket = db.Tickets.Find(Id);
            if (ticket == null)
            {
                return HttpNotFound();
            }

            var developerId = db.Roles.FirstOrDefault(d => string.Compare("Developer", d.Name, true) == 0).Id;
            var developers = db.Users.Where(r => r.Roles.Any(a => a.RoleId == developerId));
            ViewBag.AssignedUserId = new SelectList(developers, "Id", "Firstname", ticket.AssignedUserId);
            return View();

            
                //ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
                ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority", ticket.Priority);
                ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type", ticket.Type);

                return View(ticket);
            }
        




        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,OwnerId,Created,Updated,Title,Body,StatusId,AssignedUserId,StatusId,PriorityId,TypeId,ProjectId")] Tickets ticket)
        {

            if (ModelState.IsValid)
            {

                var devId = db.Roles.FirstOrDefault(d => string.Compare("Developer", d.Name, true) == 0).Id;
                var developers = db.Users.Where(r => r.Roles.Any(a => a.RoleId == devId));
                ViewBag.AssignedUserId = new SelectList(developers, "Id", "DisplayName", ticket.AssignedUserId);


                List<Tickets> projects = new List<Tickets>();
                ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status", "tickets.Status");

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


                //Tickets History 

                // create new History object
                var oldValue = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);
                var orig = "";
                var current = "";
                var message = "";
                bool modified = false;

                TicketHistory history = new TicketHistory();

                // Compare old ticket value to new ticket value
                // if ticket value has changed, indicate changes in property of ticket object

                if (oldValue.Title != ticket.Title)
                { 
                    CreateHist(oldValue.Id, "Title", oldValue.Title, ticket.Title).modified = true;
                }

                if (oldValue.Body != ticket.Body)
                {
                     CreateHist(oldValue.Id, "Body", oldValue.Body, ticket.Body).modified = true;
                }

                if (oldValue.TypeId != ticket.TypeId)
                {
                    orig = db.TicketTypes.Find(oldValue.TypeId).Type;
                    current = db.TicketTypes.Find(ticket.TypeId).Type;
                    CreateHist(oldValue.Id, "Ticket Type", orig, current).modified = true;
                }

                if (oldValue.PriorityId != ticket.PriorityId)
                {
                    orig = db.TicketPriorities.Find(oldValue.PriorityId).Priority;
                    current = db.TicketPriorities.Find(ticket.PriorityId).Priority;
                    CreateHist(oldValue.Id, "Ticket Priority", orig, current).modified = true;
                }

                if (oldValue.StatusId != ticket.StatusId)
                {
                    orig = db.TicketStatus.Find(oldValue.StatusId).Status;
                    current = db.TicketStatus.Find(ticket.StatusId).Status;
                    CreateHist(oldValue.Id, "Ticket Status", orig, current).modified = true;
                }

                //Ticket Notification

                if (oldValue.AssignedUserId != ticket.AssignedUserId)
                {
                    if (oldValue.AssignedUserId != null)
                    {
                        orig = db.Users.Find(oldValue.AssignedUserId).DisplayName;
                    }
                    if (ticket.AssignedUserId != null)
                    {
                        current = db.Users.Find(ticket.AssignedUserId).DisplayName;
                    }

                    CreateHist(oldValue.Id, "Assigned To", orig, current);
                    message = "A Ticket Has Been Assigned to You";
                 }

                db.Entry(ticket).State = EntityState.Modified;
                ticket.Updated = DateTimeOffset.Now;
                db.Tickets.Add(ticket);
                db.TicketHistory.Add(history);
                db.SaveChanges();

                //Notify assignedUser of modification
                if (modified)
                {

                    if (ticket.AssignedUserId != null)
                    {
                        if (message == "")
                        {
                            message = "A Ticket Assigned to You Has Been Modified";
                        }
                        sendemail(ticket.AssignedUserId, ticket.Id, message);
                    }
                }
            }
            return RedirectToAction("Index");


            ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status", ticket.Status);
            //ViewBag.AssignedUserId = new SelectList(developers, "Id", "DisplayName", ticket.AssignedUserId);
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority", ticket.Priority);
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type", ticket.Type);
            return View(ticket);
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







