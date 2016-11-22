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
using System.Threading.Tasks;

namespace BugTracker2.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
 

        //GET: Tickets
        public ActionResult Index(string userId, int? Id, string AssignedUserId)
        {
            Projects projects = db.Projects.Find(Id);
            userId = User.Identity.GetUserId();
            TicketsHelper helper = new TicketsHelper(db);
            var ticket = helper.GetUserTickets(userId);
            ViewBag.AssignedUserId = new SelectList(db.Users, "Id", "Firstname");
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority");
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type");
            return View(ticket);
         }

        // GET: Tickets/FullList/5
        [Authorize(Roles = "Admin")]
        public ActionResult FullList(int? Id)
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
        //[Authorize(Roles = "Admin, Submitter")]
        public ActionResult Create(int Id)
        { 
            ViewBag.ProjectId = Id;
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority");
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type");
            ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status");
            ViewBag.AssignedUserId = Id;
            return View();
        }


        //POST: Tickets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Body,ProjectId,TypeId,PriorityId,AssignedUserId,AssignedUser")] Tickets ticket)
        {
            if (ModelState.IsValid)
            {
                ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority", ticket.Priority);
                ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type", ticket.Type);
                ticket.Created = DateTimeOffset.Now;

                ticket.StatusId = db.TicketStatus.SingleOrDefault(s => s.Status == "Open").Id;
                ticket.AssignedUserId = User.Identity.GetUserId();
                ticket.OwnerId = User.Identity.GetUserId();
                db.Tickets.Add(ticket);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
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

            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority", ticket.Priority);
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type", ticket.Type);
            ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status", ticket.Status);
            return View(ticket);
            }


        // POST: Tickets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,OwnerId,Created,Updated,Title,Body,AssignedUserId,StatusId,PriorityId,TypeId,ProjectId,TicketId")] Tickets ticket)
        {

            if (ModelState.IsValid)
            {

                //List<Tickets> projects = new List<Tickets>();
                //ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status", ticket.Status);

                //if (User.IsInRole("Admin"))
                //{
                //    var UserId = User.Identity.GetUserId();
                //    ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority", "tickets.Priority");
                //}

                //if (User.IsInRole("ProjectManager") || User.IsInRole("Developer") || User.IsInRole("Submitter"))
                //{
                //    var UserId = User.Identity.GetUserId();
                //    ViewBag.PriorityId = new SelectList(db.TicketPriorities.Where(p => p.Priority == UserId)).ToList();
                //}

                ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type", ticket.Type);
                ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status", ticket.Status);

                var ticketPriority = db.TicketPriorities.FirstOrDefault(t => t.Id == ticket.Id);
                ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority", ticket.Priority);

                var ticketType = db.TicketTypes.FirstOrDefault(t => t.Id == ticket.Id);
                ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type", ticket.Type);

                var devId = db.Roles.FirstOrDefault(d => string.Compare("Developer", d.Name, true) == 0).Id;
                var developers = db.Users.Where(r => r.Roles.Any(a => a.RoleId == devId));
                ViewBag.AssignedUserId = new SelectList(developers, "Id", "DisplayName", ticket.AssignedUserId);

                ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);

                var ticketId = db.Projects.FirstOrDefault(p => p.Id == ticket.Id);
                db.Tickets.Attach(ticket);
                ticket.Updated = DateTimeOffset.Now;
                db.Entry(ticket).State = EntityState.Modified;
                db.SaveChanges();
            }


            //var oldValue = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == ticket.Id);  
            //TicketHistory  history = new TicketHistory();
            //var newValue = "";
            //var orig = "";
            //var current = "";
            //var message = "";

            //bool modified = false;

            //history.HistoryUserId = User.Identity.GetUserId();


            // Compare old ticket value to new ticket value
            // if ticket value has changed, indicate changes in property of ticket object

            //if (oldValue.Title != ticket.Title)
            //{
            //    CreateHist(oldValue.Id, "Title", oldValue.Title, ticket.Title);
            //    modified = true;
            //}

            //if (oldValue.Body != ticket.Body)
            //{
            //    CreateHist(oldValue.Id, "Body", oldValue.Body, ticket.Body);
            //    modified = true;
            //}

            //if (oldValue.TypeId != ticket.TypeId)
            //{
            //    orig = db.TicketTypes.Find(oldValue.TypeId).Type;
            //    current = db.TicketTypes.Find(ticket.TypeId).Type;
            //    History(oldValue.Id, "Ticket Type", orig, current);
            //    modified = true;
            //}

            //if (oldValue.PriorityId != ticket.PriorityId)
            //{
            //    orig = db.TicketPriorities.Find(oldValue.PriorityId).Priority;
            //    current = db.TicketPriorities.Find(ticket.PriorityId).Priority;
            //    CreateHist(oldValue.Id, "Ticket Priority", orig, current);
            //    modified = true;
            //}

            //if (oldValue.StatusId != ticket.StatusId)
            //{
            //    orig = db.TicketStatus.Find(oldValue.StatusId).Status;
            //    current = db.TicketStatus.Find(ticket.StatusId).Status;
            //    CreateHist(oldValue.Id, "Ticket Status", orig, current);
            //    modified = true;
            //}


            //ticket.Updated = DateTimeOffset.Now;
            //db.Entry(ticket).State = EntityState.Modified;
            //db.Tickets.Add(ticket);
            //db.TicketHistory.Add(history);
            //db.SaveChanges();


            ////Send Notification
            ////var tickets = db.Tickets.Find(Id);
            //var developer = db.Users.Find(ticket.AssignedUserId);
            //if (developer != null && developer.Email != null)
            //{
            //    var svc = new EmailService();
            //    var msg = new IdentityMessage();
            //    msg.Destination = developer.Email;
            //    msg.Subject = "Bug Tracker Update: " + ticket.Title;
            //    msg.Body = ("The following changes have been made to Ticket  " + ticket.Id + " - " + ticket.Title + ": " + message);
            //    await svc.SendAsync(msg);

            return RedirectToAction("Index");
            {

                ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status", ticket.Status);
                ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority", ticket.Priority);
                ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type", ticket.Type);
                return View(ticket);
            }
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







