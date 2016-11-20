using BugTracker2.Models;
using BugTracker2.Models.Helper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
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
        public ActionResult Index()
        {
            DashboardViewModel model = new DashboardViewModel();
            List<Projects> projects = new List<Projects>();
            List<Tickets> ticket = new List<Tickets>();
            TicketsHelper helper = new TicketsHelper(db);

            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager") || User.IsInRole("Developer") || User.IsInRole("Submitter"))
            {
                var UserId = User.Identity.GetUserId();
                projects = db.Projects.Where(p => p.Users.Any(u => u.Id == UserId)).Take(3).ToList();
                ticket = db.Tickets.Where(t => t.AssignedUser.Any(u => u.AssignedUserId == UserId)).Take(3).ToList();
            }   
            return View();
          }
            

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
