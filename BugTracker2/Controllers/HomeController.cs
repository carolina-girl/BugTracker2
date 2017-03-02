using BugTracker2.Models;
using BugTracker2.Models.Helper;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace BugTracker2.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private UserManager<ApplicationUser> manager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));

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
                ProjectsHelper phelper = new ProjectsHelper(db);
                var project = phelper.ListProjects(UserId).ToList();
                model.Projects = user.Projects.OrderByDescending(p => p.Created).ToList();

                var userId = User.Identity.GetUserId();
                TicketsHelper helper = new TicketsHelper(db);
                model.Tickets = helper.GetUserTickets(userId);
            }
            ViewBag.Message = "The dashboard shows the  elements of the projects and tickets page. Click on dashboard/tickets tab to view tickets.";
            //return the projects and tickets model to the view               
            model.OpenTickets = db.Tickets.Where(t => t.Status.Status == "Open").AsNoTracking().ToList();
            model.PendingTickets = db.Tickets.Where(t => t.Status.Status == "Pending").AsNoTracking().ToList();
            model.ClosedTickets = db.Tickets.Where(t => t.Status.Status == "Closed").AsNoTracking().ToList();
            return View(model);

        }
       

        //GET: Index
        public ActionResult Index(int? Id)
        {
            return RedirectToAction("Login", "Account");
        }


        // GET: /Home/UserProfile
        [Authorize]
        public ActionResult UserProfile(string userId)
        {
            var user = manager.FindById(User.Identity.GetUserId());
            userId = User.Identity.GetUserId();
            UserViewModel model = new UserViewModel();
            UserRoleAssignHelper helper = new UserRoleAssignHelper(db);

            model.Name = user.FullName;
            model.Roles = helper.ListMyRoles(userId);
            model.ProjectCount = db.Projects.Count();
            model.Email = user.Email;
            var tickets = user.Projects.SelectMany(p => p.Tickets).ToList();
            model.TicketsAssigned = tickets.Where(t => t.AssignedUserId == user.Id).Count();
            model.TicketsSubmitted = tickets.Where(t => t.OwnerId == user.Id).Count();

            return View(model);
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
    

