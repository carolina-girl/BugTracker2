using BugTracker2.Models;
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
            List<Projects> projects = new List<Projects>();
            ViewBag.Projects = new SelectList(db.Projects, "Id", "Projects");
            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager") || User.IsInRole("Developer") || User.IsInRole("Submitter"))
            {
                var UserId = User.Identity.GetUserId();
                projects = db.Projects.Where(p => p.Users.Any(u => u.Id == UserId)).ToList();
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
