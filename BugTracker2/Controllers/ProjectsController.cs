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

namespace BugTracker2.Controllers
{
    public class ProjectsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Projects
        public ActionResult Index()
        {
            List<Projects> projects = new List<Projects>();

            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager") || User.IsInRole("Developer") || User.IsInRole("Submitter"))
            {
                var UserId = User.Identity.GetUserId();
                projects = db.Projects.Where(p => p.Users.Any(u => u.Id == UserId)).ToList();
            }
            
                return View(projects);
            }
        

        // GET: Projects
        [Authorize(Roles = "Admin,ProjectManager")]
        public ActionResult FullList()
        {
            List<Projects> projects = new List<Projects>();
            if (User.IsInRole("Admin") || User.IsInRole("ProjectManager"))
            {
                projects = db.Projects.ToList();
            }
            
                return View(projects);
            }
        

        [Authorize(Roles = "Admin,ProjectManager,Developer,Submitter")]
        // GET: Projects/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }



        // GET: Projects/Create
        [Authorize(Roles = "Admin,ProjectManager")]
        public ActionResult Create()
        {
            return View();
        }



        // POST: Projects/Create
        [Authorize(Roles = "Admin,ProjectManager")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlin.k/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Created,Updated,Body")] Projects projects)
        {
            if (ModelState.IsValid)
            {
                projects.Updated = DateTimeOffset.Now;
                string userId = User.Identity.GetUserId();
                db.Projects.Add(projects);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(projects);
        }


        // GET: Projects/Edit/5
        [Authorize(Roles = "Admin,ProjectManager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }


        // POST: Projects/Edit/5
        [Authorize(Roles = "Admin,ProjectManager")]
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Created,Updated,Body")] Projects projects)
        {
            if (ModelState.IsValid)
            {
                projects.Updated = DateTimeOffset.Now;
                db.Entry(projects).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(projects);
        }

        // GET: Projects/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Projects projects = db.Projects.Find(id);
            if (projects == null)
            {
                return HttpNotFound();
            }
            return View(projects);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            Projects projects = db.Projects.Find(id);
            db.Projects.Remove(projects);
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
    

