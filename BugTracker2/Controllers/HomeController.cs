using BugTracker2.Models;
using BugTracker2.Models.Helper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace BugTracker2.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //GET: Home/Dashboard/
        public ActionResult Dashboard(int? Id)
        {  
            //object of dashboardViewModel
            DashboardViewModel model = new DashboardViewModel();
            //get user Id
            var UserId = User.Identity.GetUserId();
            var user = db.Users.Find(UserId);
            //filter list of projects according to user role
            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager") || User.IsInRole("Developer") || User.IsInRole("Submitter"))
            {
                //list this user's projects
                var project = new List<project>();
                ProjectsHelper phelper = new ProjectsHelper(db);
                project = user.Projects.ToList();
                //list tickets for these projects
                TicketsHelper thelper = new TicketsHelper(db);
                var tickets = thelper.GetUserTickets(UserId).ToList();
             }
 

            //goes into a project db and takes the top 5 of a list and order it by desc of name.
            //and returns it to the projects model
            model.Projects = db.Projects.OrderByDescending(p => p.Title).Take(5).ToList();
            //goes into a ticket db and takes the top 10 of a list and order it by desc of name.
            //and returns it to the tickets model
            model.Tickets = db.Tickets.OrderByDescending(t => t.Updated).Take(10).ToList();

            ViewBag.Message = "The dashboard shows the most recent elements of the projects and tickets page. Click on dashboard/tickets tab to view new recent tickets.";
            //return the projects and tickets model to the view
            return View(model);
     
          }
       

        //GET: Index
        public ActionResult Index(int? Id)
        {
            return RedirectToAction("Login", "Account");
        }

        // GET: User Profile
        public ActionResult UserProfile()
        {
            ApplicationUser user = db.Users.Find(TempData["UserId"]);
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            UserViewModel model = new UserViewModel();
            project project = db.Projects.Find(TempData["ProjectId"]);
            model.ProjectId = project.Id;
            UserRoleAssignHelper helper = new UserRoleAssignHelper(db);
            model.Name = user.FullName;
            model.Email = user.Email;
            model.PhoneNumber = user.PhoneNumber;
            model.ProjectCount = user.Projects.Count();
            model.Roles = helper.ListMyRoles(user.Id);
            var tickets = user.Projects.SelectMany(p => p.Tickets).ToList();
            model.TicketsAssigned = tickets.Where(t => t.AssignedUserId == user.Id).Count();
            model.TicketsSubmitted = tickets.Where(t => t.OwnerId == user.Id).Count();
            return View(model);
        }

        [HttpPost]
        public ActionResult UserProfile(string userId, int? ProjectId)
        {
            var user = db.Users.Find(userId);
            TempData["UserId"] = userId;
            TempData["ProjectId"] = ProjectId;
            return RedirectToAction("UserProfile");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
    

