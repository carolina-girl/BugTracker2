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
        //    {
        //        DashboardViewModel model = new DashboardViewModel();
        //        model.Id = User.Identity.GetUserId();
        //        var user = db.Users.Find(model.Id);
        //        model.Name = user.FirstName + " " + user.LastName;
        //        model.Projects = user.Projects.Where(p => p.Archived == false).ToList();

        //        TicketsHelper helper = new TicketsHelper(db);
        //        model.Tickets = helper.GetUserTickets(model.Id).Where(t => t.Status != Status.Closed).OrderByDescending(t => t.Created).Take(5).ToList();
        //        return View(model);
        //    }
        //}


        DashboardViewModel model = new DashboardViewModel();
        List<Projects> projects = new List<Projects>();
        List<Tickets> tickets = new List<Tickets>();
        //TicketsHelper helper = new TicketsHelper(db);

        IEnumerable<Tickets> Tickets = db.Tickets.AsQueryable();

            {
                model.Id = User.Identity.GetUserId();
                var user = db.Users.Find(model.Id);
                var project = db.Projects.Find(Id);
        var ticket = db.Tickets.Find(Id);
        var UserId = User.Identity.GetUserId();
        projects = db.Projects.Where(p => p.Users.Any(u => u.Id == UserId)).Take(3).ToList();
        tickets = db.Tickets.Where(t => t.Users.Any(u => u.Id == UserId)).Take(3).ToList();
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
    

