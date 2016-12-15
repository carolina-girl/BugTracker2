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

        //GET: Index
        public ActionResult Index(int? Id)
        {
            {
                DashboardViewModel model = new DashboardViewModel();
                model.Id = User.Identity.GetUserId();
                var user = db.Users.Find(model.Id);
                model.Name = user.FullName + " " + user.LastName;
                model.Projects = user.Projects.ToList();

                TicketsHelper helper = new TicketsHelper(db);
                model.Tickets = helper.GetUserTickets(model.Id).OrderByDescending(t => t.Created).Take(3).ToList();
                return View(model);
            }
        }

        // GET: User Profile
        public ActionResult UserProfile()
        {
            ApplicationUser user = db.Users.Find(TempData["UserId"]);
            if (user == null)
            {
                return RedirectToAction("Index");
            }
            UserProfileView model = new UserProfileView();
            Projects project = db.Projects.Find(TempData["ProjectId"]);
            model.ProjectId = project.Id;
            UserRoleAssignHelper helper = new UserRoleAssignHelper();
            model.Name = user.FullName;
            model.Email = user.Email;
            model.PhoneNumber = user.PhoneNumber;
            model.ProjectCount = user.Projects.Count();
            //model.Roles = helper.ListUserRoles();
            var tickets = user.Projects.SelectMany(p => p.Tickets).ToList();
            model.TicketsAssigned = tickets.Where(t => t.AssignedUserId == user.Id).Count();
            model.TicketsSubmitted = tickets.Where(t => t.OwnerId == user.Id).Count();
            //model.TicketsResolved = tickets.Where(t => t.AssignedUserId == user.Id).Where(t => t.Status == Status.Resolved || t.Status == Status.Closed).Count();
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
    

