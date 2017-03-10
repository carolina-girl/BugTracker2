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
        public ActionResult Dashboard(int? Id, int? projectId)
        {
            DashboardViewModel model = new DashboardViewModel();              
            //get user Id
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            //filter list of projects according to user role
            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager") || User.IsInRole("Developer") || User.IsInRole("Submitter"))
            {
                //list this user's projects
                ProjectsHelper phelper = new ProjectsHelper(db);
                var project = phelper.ListProjects(userId).ToList();
                model.Projects = user.Projects.OrderByDescending(p => p.Created).ToList();
                //list this user's tickets
                TicketsHelper helper = new TicketsHelper(db);
                model.Tickets = helper.GetUserTickets(userId);
                var ticket = helper.ListTickets(userId).ToList();
                model.OpenTickets = user.Projects.SelectMany(p => p.Tickets).Where(t => t.Status.Status == "Open").ToList();
                model.PendingTickets = user.Projects.SelectMany(p => p.Tickets).Where(t => t.Status.Status == "Pending").ToList();
                model.ClosedTickets = user.Projects.SelectMany(p => p.Tickets).Where(t => t.Status.Status == "Closed").ToList();
            }            
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
    

