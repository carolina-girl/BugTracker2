﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using BugTracker2.Models;
using Microsoft.AspNet.Identity;
using BugTracker2.Models.Helper;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Threading.Tasks;
using System.Text;
using BugTracker2.Helper;
using System.Configuration;
using SendGrid;
using System.Net.Mail;
using BugTracker2.Controllers;

namespace BugTracker2.Controllers
{
    public class TicketsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

        [Authorize(Roles = "Admin, Submitter, Developer, ProjectManager")]
        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            TicketsHelper helper = new TicketsHelper(db);
            var tickets = helper.GetUserTickets(userId);

            return View(tickets);
        }


        // GET: Tickets/FullList/5
        [Authorize(Roles = "Admin")]
        public ActionResult FullList(int? Id)
        {
            return View(db.Tickets.ToList());
        }


        // GET: Tickets/Details/5
        [Authorize(Roles = "Admin, Submitter, Developer, ProjectManager")]
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
            PermissionHelper permissionHelper = new PermissionHelper(db);
            ViewBag.UserId = User.Identity.GetUserId();

            if (permissionHelper.HasTicketPermission(ViewBag.UserId, ticket.Id))
            {
                ViewBag.HasTicketPermission = true;
                return View(ticket);
            }
            ModelState.AddModelError("hello", "You do not have permission to view this ticket.");
            ViewBag.HasTicketPermission = false;
            return View((Tickets)null);
        }



        //POST: Tickets/Details/5
        [Authorize(Roles = "Admin, Submitter, Developer, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details([Bind(Include = "Id,Title,OwnerId,Body,Created,Updated,ProjectId")] Tickets ticket)
        {
            List<Tickets> Ticket = new List<Tickets>();
            var assignedUser = ticket.AssignedUserId;
            ViewBag.AssignedUserId = new SelectList(db.Users, "Developer", "FullName");

            return RedirectToAction("Index");
        }

        // GET: Tickets/History/5
        [Authorize]
        public ActionResult History(int? id)
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
            TicketsHelper ticketHelper = new TicketsHelper(db);
            var userId = User.Identity.GetUserId();
            if (ticketHelper.HasTicketPermission(userId, ticket.Id))
            {
                ViewBag.UserId = User.Identity.GetUserId();
                return View(ticket);
            }
            TempData["Error"] = "Sorry, you do not have permission to view that ticket.";
            return RedirectToAction("Index");
        }

        // GET: Tickets/Create
        [Authorize(Roles = "Submitter")]
        public ActionResult Create(int? Id, string AssignedUserId)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects project = db.Projects.Find(Id);
            if (project == null)
            {
                return HttpNotFound();
            }

            ViewBag.OwnerId = new SelectList(db.Users, "Id", "Owner");
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority");
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type");
            ViewBag.ProjectId = Id;
            ViewBag.ProjectTitle = project.Title;
            return View();
        }
        


        //POST: Tickets/Create
        [Authorize(Roles = "Submitter")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Title,Body,ProjectId,OwnerId,PriorityId,TypeId,AssignedUserId,Type,Priority")] Tickets ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.Created = System.DateTimeOffset.Now;
                ticket.OwnerId = User.Identity.GetUserId();

                ticket.StatusId = db.TicketStatus.SingleOrDefault(s => s.Status == "Open").Id;
                ticket.AssignedUserId = User.Identity.GetUserId();
                ticket.OwnerId = User.Identity.GetUserId();

                TicketHistory history = new TicketHistory();
                history.Date = ticket.Created;
                var historyBody = "Ticket created <br> Title: " + ticket.Title + "<br> Body: " + ticket.Body + "<br> Priority: " + ticket.Priority + "<br> Type: " + ticket.Type;
                history.Body = historyBody;
                history.Body = historyBody.ToString();
                history.TicketId = ticket.Id;
                db.Tickets.Add(ticket);
                db.TicketHistory.Add(history);
                db.SaveChanges();


                var user = db.Users.Find(ticket.OwnerId);
                TicketNotify notify = new TicketNotify();
                notify.TicketId = ticket.Id;
                notify.NotifyUserId = ticket.OwnerId;
                db.Notifications.Add(notify);
                UserRoleAssignHelper helper = new UserRoleAssignHelper();
                var manager = db.ProjectUsers.Where(p => p.ProjectId == ticket.ProjectId).ToList();
                foreach (var item in manager)
                {
                    if (helper.IsUserInRole(item.ProjectUserId, "ProjectManager"))
                    {
                        TicketNotify notify2 = new TicketNotify();
                        notify2.TicketId = ticket.Id;
                        notify.NotifyUserId = item.ProjectUserId;
                        db.Notifications.Add(notify2);
                    }
                }
                var emails = db.Notifications.Where(t => t.TicketId == ticket.Id).Select(u => u.NotifyUser.Email).ToList();
                var apiKey = ConfigurationManager.AppSettings["SendGridAPIkey"];
                var from = ConfigurationManager.AppSettings["ContactEmail"];

                foreach (var email in emails)
                {
                    SendGridMessage myMessage = new SendGridMessage();
                    myMessage.AddTo(email);
                    myMessage.From = new MailAddress(from);
                    myMessage.Subject = "Notification for Ticket #" + ticket.Id;
                    myMessage.Html = "Ticket #" + ticket.Id + " has been created";
                    var credentials = new NetworkCredential(apiKey, from);

                    var transportWeb = new Web(credentials);

                    await transportWeb.DeliverAsync(myMessage);
                }
                return RedirectToAction("Index");
            }

            ViewBag.OwnerId = new SelectList(db.Tickets, "Id", "Owner", ticket.OwnerId);
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Project", ticket.ProjectId);
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority", ticket.Priority);
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type", ticket.Type);
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
            PermissionHelper permissionHelper = new PermissionHelper(db);
            ViewBag.UserId = User.Identity.GetUserId();
            if (!permissionHelper.HasTicketPermission(ViewBag.UserId, ticket.Id))
            {
                ModelState.AddModelError("Sorry,", "You do not have permission to view this ticket.");
                ViewBag.HasTicketPermission = false;
                return RedirectToAction("Index");
            }

            ticket = db.Tickets.Find(Id);
            var developerId = db.Roles.FirstOrDefault(d => string.Compare("Developer", d.Name, true) == 0).Id;
            var developers = db.Users.Where(r => r.Roles.Any(a => a.RoleId == developerId));
            ViewBag.AssignedUserId = new SelectList(developers, "Id", "FullName", ticket.AssignedUserId);

            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority", ticket.PriorityId);
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type", ticket.TypeId);
            ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status", ticket.Status);

            var project = db.Projects.FirstOrDefault(p => p.Id == ticket.ProjectId);
            var ProjectTitle = project.Title;
            var type = db.TicketTypes.Find(ticket.TypeId);
            var TicketType = type;
            ViewBag.ProjectTitle = ProjectTitle;
            ViewBag.TicketType = TicketType;
            if (User.IsInRole("Developer") || User.IsInRole("ProjectManager"))
            {
                ticket = db.Tickets.Find(Id);
                ViewBag.StatusId = new SelectList(db.TicketStatus, "Id", "Status", ticket.StatusId);
            }
            return View(ticket);
        }




        // POST: Tickets/Edit/5
        [Authorize(Roles = "Admin, Submitter, Developer, ProjectManager")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,OwnerId,UserId,Created,Updated,Title,Body,AssignedUserId,StatusId,PriorityId,TypeId,TicketId,ProjectId")] Tickets ticket)
        {
            if (ModelState.IsValid)
            {
                ticket.StatusId = db.TicketStatus.SingleOrDefault(s => s.Status == "Pending").Id;
                //ticket history 
                ticket.Updated = DateTimeOffset.Now;
                var userId = User.Identity.GetUserId();
                var user = db.Users.Find(userId);
                var oldValue = db.Tickets.AsNoTracking().Include("AssignedUser").FirstOrDefault(t => t.Id == ticket.Id);
                if (oldValue.Title != ticket.Title || 
                    oldValue.Body != ticket.Body || 
                    oldValue.TypeId != ticket.TypeId || 
                    oldValue.PriorityId != ticket.PriorityId ||
                    oldValue.AssignedUserId != ticket.AssignedUserId)
                {
                    TicketHistory history = new TicketHistory();
                    history.Date = DateTimeOffset.Now;
                    StringBuilder historyBody = new StringBuilder();
                    historyBody.Append("Ticket edited by  ");
                    historyBody.Append(user.FullName);
                    Comments comment = new Comments();
                    historyBody.Append("A comment was add on  ");
                    historyBody.Append(comment.Created);

                    if (oldValue.Title != ticket.Title)
                    {
                        historyBody.AppendFormat(" The title was changed from {0} to {1}. ", oldValue.Title, ticket.Title);
                    }
                    if (oldValue.Body != ticket.Body)
                    {
                        historyBody.AppendFormat(" The body was changed from {0} to {1}. ", oldValue.Body, ticket.Body);
                    }
                    if (oldValue.TypeId != ticket.TypeId)
                    {
                        var oldTypeName = db.TicketTypes.Find(oldValue.TypeId).Type;
                        var currTypeName = db.TicketTypes.Find(ticket.TypeId).Type;
                        if (oldTypeName != currTypeName)
                        {
                            historyBody.AppendFormat(" The type was changed from {0} to {1}. ", oldTypeName, currTypeName);
                        }
                    }
                    if (oldValue.PriorityId != ticket.PriorityId)
                    {
                        var oldPriorityName = db.TicketPriorities.Find(oldValue.PriorityId).Priority;
                        var currPriorityName = db.TicketPriorities.Find(ticket.PriorityId).Priority;
                        if (oldPriorityName != currPriorityName)
                        {
                            historyBody.AppendFormat(" The priority was changed from {0} to {1}. ", oldPriorityName, currPriorityName);
                        }
                    }
                
                    //Attachments attachments = new Attachments();
                    //db.Attachments.Add(attachments);
                    //attachments = db.Attachments.Find(attachments.Id);
                    //historyBody.AppendFormat("  An attachment has been added to this ticket.");
                    //Comments comment = new Comments();
                    //db.Comments.Add(comment);
                    //comment = db.Comments.Find(comment.Id);                
                    //historyBody.AppendFormat("  A comment has been added to this ticket.");

                    history.Body = historyBody.ToString();
                    history.TicketId = ticket.Id;
                    db.TicketHistory.Add(history);
                    db.Tickets.Attach(ticket);
                    db.Entry(ticket).State = EntityState.Modified;
                    db.Entry(ticket).Property(x => x.Created).IsModified = false;
                    db.Entry(ticket).Property(x => x.OwnerId).IsModified = false;
                    db.Entry(ticket).Property(x => x.ProjectId).IsModified = false;
                    db.Entry(ticket).Property(x => x.userId).IsModified = false;
                    db.Entry(ticket).Property("Updated").IsModified = true;
                    db.Entry(ticket).Property("Title").IsModified = true;
                    db.Entry(ticket).Property("Body").IsModified = true;
                    db.SaveChanges();
                }
                else
                {
                    ModelState.AddModelError("", "Error: No changes have been made.");
                    return View(ticket);
                }

                //Send Notification
                var tickets = db.Tickets.Find(ticket.Id);
                var developer = db.Users.Find(ticket.AssignedUserId);
                if (developer != null && developer.Email != null)
                {
                    var svc = new EmailService();
                    var msg = new IdentityMessage();
                    msg.Destination = developer.Email;
                    msg.Subject = "Bug Tracker Update: " + ticket.Title;
                    msg.Body = ("The following changes have been made to Ticket  " + ticket.Id + " - " + ticket.Title + ": " + msg);
                    await svc.SendAsync(msg);
                } //await NotifyDeveloper(ticket.Id, userId, ticket.AssignedUserId);
                return RedirectToAction("Index", "Tickets", new { id = ticket.Id });
            }

            // var ticketPriority = db.TicketPriorities.FirstOrDefault(t => t.Id == ticket.Id);
            ViewBag.PriorityId = new SelectList(db.TicketPriorities, "Id", "Priority", ticket.PriorityId);

            // var ticketType = db.TicketTypes.FirstOrDefault(t => t.Id == ticket.Id);
            ViewBag.TypeId = new SelectList(db.TicketTypes, "Id", "Type", ticket.TypeId);

            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "Title", ticket.ProjectId);

            //var developerId = db.Roles.FirstOrDefault(d => string.Compare("Developer", d.Name, true) == 0).Id;
            //var developers = db.Users.Where(r => r.Roles.Any(a => a.RoleId == developerId));
            //ViewBag.AssignedUserId = new SelectList(developers, "Id", "FullName", ticket.AssignedUserId);
           
            return View(ticket);
        }

        //GET Tickets/Assign Users
        [Authorize(Roles = "Admin,ProjectManager")]
        public ActionResult AssignUser(int? Id)
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
            AssignTicketsViewModel AssignModel = new AssignTicketsViewModel();
            AssignModel.TicketId = ticket.Id;
            AssignModel.TicketTitle = ticket.Title;
            ProjectsHelper helper = new ProjectsHelper(db);
            UserRoleAssignHelper userHelper = new UserRoleAssignHelper(db);
            var projectUsers = helper.ListUsers(ticket.ProjectId);
            var projectDevelopers = new List<ApplicationUser>();
            foreach (var user in projectUsers)
            {
                if (userHelper.IsUserInRole(user.Id, "Developer"))
                {
                    projectDevelopers.Add(user);
                }
            }
            if (ticket.AssignedUser != null)
            {
                AssignModel.TicketAssignedTo = ticket.AssignedUser.FullName;
            }
            AssignModel.UsersList = new SelectList(projectDevelopers, "Id", "FullName");
            return View(AssignModel);
        }

        ////POST: Tickets/AssignUser
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignUser(string UserId, int TicketId)
        {
            //Ticket details
            var oldTicket = db.Tickets.AsNoTracking().FirstOrDefault(t => t.Id == TicketId);
            var ticket = db.Tickets.Find(TicketId);
            ticket.AssignedUserId = UserId;
            ticket.Created = System.DateTimeOffset.Now;
            //Add to ticket history
            TicketHistory history = new TicketHistory();
            history.Date = DateTimeOffset.Now;
            var user = db.Users.Find(UserId);
            var historyBody = "Ticket assigned to " + user.FullName + ". Ticket now Pending.";
            history.Body = historyBody;
            history.TicketId = ticket.Id;
            db.TicketHistory.Add(history);
            //send email to previous developer
            if (oldTicket.AssignedUserId != ticket.AssignedUserId && oldTicket.AssignedUserId == null)
            {
                var svc2 = new EmailService();
                var msg2 = new IdentityMessage();
                msg2.Destination = user.Email;
                msg2.Subject = "Bug Tracker: Ticket Reassigned";
                msg2.Body = ticket.Owner.FullName + " has reassigned the ticket '" + ticket.Title + "' to another developer. You are no longer responsible for this ticket. If you have any questions regarding this ticket, " + ticket.Owner.FullName + " can be contacted at " + ticket.Owner.Email;
                await svc2.SendAsync(msg2);
            }
            if (oldTicket.AssignedUserId == ticket.AssignedUserId)
            {
                ModelState.AddModelError("", "Error: No changes have been made.");
                return RedirectToAction("AssignUser", new { id = TicketId });
            }
            //Save to database
            db.Tickets.Attach(ticket);
            db.Entry(ticket).Property("AssigneduserId").IsModified = true;
            db.Entry(ticket).Property("Status").IsModified = true;
            db.Entry(ticket).Property("Created").IsModified = true;
            db.SaveChanges();
            //Send email to developer assigned
            var svc = new EmailService();
            var msg = new IdentityMessage();
            msg.Destination = user.Email;
            msg.Subject = "Bug Tracker: New Ticket Assigned";
            msg.Body = ticket.Owner.FullName + " has assigned you the ticket '" + ticket.Title + "'.";
            await svc.SendAsync(msg);

            return RedirectToAction("Details", new { id = TicketId });
        }


        // GET: Tickets/Close/5
        [Authorize(Roles = "Developer, ProjectManager")]
        public ActionResult Close(int? Id)
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

        // POST: Tickets/Close
        [Authorize(Roles = "Developer, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Close(int Id, string history)
        {
            Tickets ticket = db.Tickets.Find(Id);
            ticket.StatusId = db.TicketStatus.SingleOrDefault(s => s.Status == "Closed").Id;
            ticket.Updated = DateTimeOffset.Now;
            var NotifyUserId = User.Identity.GetUserId();
            var user = db.Users.Find(NotifyUserId);
            TicketHistory hist = new TicketHistory();
            hist.Date = System.DateTimeOffset.Now;
            var historyBody = "Ticket Closed by " + user.FullName;
            hist.Body = historyBody;
            hist.TicketId = ticket.Id;
            db.TicketHistory.Add(hist);
            db.Tickets.Attach(ticket);
            db.Entry(ticket).Property("StatusId").IsModified = true;
            db.Entry(ticket).Property("Updated").IsModified = true;
            var tickets = db.Tickets.Find(ticket.Id);
            var developer = db.Users.Find(ticket.AssignedUserId);
            if (developer != null && developer.Email != null)
            {
                var svc = new EmailService();
                var msg = new IdentityMessage();
                msg.Destination = developer.Email;
                msg.Subject = "Bug Tracker Update: " + ticket.Title;
                msg.Body = ("The following changes have been made to Ticket  " + ticket.Id + " - " + ticket.Title + ": " + msg);
                await svc.SendAsync(msg);
                //await TicketNotifyDeveloper(ticket.Id, NotifyUserId, ticket.AssignedUserId);
            }
            db.SaveChanges();

            //await TicketNotifyDeveloper(Id, NotifyUserId, ticket.AssignedUserId);
            return RedirectToAction("Index");
        }


        //GET: Tickets/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tickets tickets = db.Tickets.Find(Id);
            if (tickets == null)
            {
                return HttpNotFound();
            }
            return View(tickets);
        }

        //POST: Tickets/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int Id)
        {
            Tickets tickets = db.Tickets.Find(Id);
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







