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
using BugTracker2.Models.Helper;

namespace BugTracker2.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
       [Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
       public ActionResult Index()
        {

          if (User.IsInRole("Admin") || User.IsInRole("ProjectManager") || User.IsInRole("Developer") || User.IsInRole("Submitter"))
            {
                var UserId = User.Identity.GetUserId();
                var user = db.Users.Find(UserId);
                var project = new List<project>();
                ProjectsHelper helper = new ProjectsHelper(db);
                project = user.Projects.ToList();
                db.SaveChanges();

                return View(project);
            }

            return View();

        }


        // GET: Projects
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult FullList(int? Id)
        {
            List<project> projects = new List<project>();
            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
            {
                project project = db.Projects.Find(Id);
                projects = db.Projects.ToList();
            }
            
                return View(projects);
            }


        [Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
        // GET: Projects/Details/5
        public ActionResult Details(int? Id, string userId)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            project project = db.Projects.Find(Id);
            if (project == null)
            {
                return HttpNotFound();
            }
            var tickets = project.Tickets.OrderByDescending(t => t.Created).ToList();
            ProjectsHelper projectsHelper = new ProjectsHelper(db);
            userId = User.Identity.GetUserId();
            if (!projectsHelper.HasProject(userId, project.Id))
            {
                TempData["Error"] = "Sorry, you do not have permission to access that project.";
                return RedirectToAction("Index");
            }
            return View(project);
        }

        // GET: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        [Authorize(Roles = "Admin, ProjectManager")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlin.k/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Created,Updated,Body,UserId")] project project)
        {
            if (ModelState.IsValid)
            {
                ProjectsHelper helper = new ProjectsHelper(db);
                var UserId = User.Identity.GetUserId();
                var user = db.Users.Find(UserId);
                project.Created = DateTimeOffset.Now;
                db.Projects.Add(project);
                helper.AssignedUser(UserId, project.Id);
                db.Projects.Add(project);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }


        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Edit(int? Id)
        { 
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            project project = db.Projects.Find(Id);
            if (project == null)
            {
                return HttpNotFound();
            }
            ProjectsHelper projectsHelper = new ProjectsHelper(db);
            var userId = User.Identity.GetUserId();
            if (!projectsHelper.HasProject(userId, project.Id))
            {
                TempData["Error"] = "Sorry, you do not have permission to access that project.";
                return RedirectToAction("Index");
            }
            return View(project);
        }


        // POST: Projects/Edit/5
        [Authorize(Roles = "Admin, ProjectManager")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Created,Updated,Body")] project project)
        {
            if (ModelState.IsValid)
            {
                project.Updated = DateTimeOffset.Now;
                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }


        // GET: Projects/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            project project = db.Projects.Find(Id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? Id)
        {
            project project = db.Projects.Find(Id);
            db.Projects.Remove(project);
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
    

