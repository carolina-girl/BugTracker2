using BugTracker2.Models;
using BugTracker2.Models.Helper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IdentityModel.Protocols.WSTrust;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SqlProviderServices = System.Data.Entity.SqlServer.SqlProviderServices;

namespace BugTracker2.Controllers
 {
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserRoleAssignHelper userManager = new UserRoleAssignHelper();
        ProjectsHelper helper = new ProjectsHelper();



        // GET: Admin
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Index()
        {
            var model = db.Users.ToList();
            return View(model);
        }


        //GET: Admin/EditUser/5
        [Authorize(Roles = "Admin")]
        public ActionResult EditUser(string Id)
        {
            var user = db.Users.Find(Id);
            AdminUserViewModel AdminModel = new AdminUserViewModel();
            var selected = userManager.ListUserRoles(Id);
            AdminModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
            AdminModel.User = user;
            return View(AdminModel);
        }

        //POST:  Admin/EditUser
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditUser(AdminUserViewModel AdminModel)
        {
            var user = db.Users.Find(AdminModel.User.Id);
            foreach (var rolermv in db.Roles.Select(r => r.Name).ToList())
            {
                userManager.RemoveUserFromRole(user.Id, rolermv);
            }

            foreach (var roleadd in AdminModel.SelectedRoles)
            {
                userManager.AddUserToRole(user.Id, roleadd);
            }
            return RedirectToAction("Index");
        }


        //GET: Admin/ProjectUser/5
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult ProjectUser(string Id)
        {
            var user = db.Users.Find(Id);
            AdminProjectUserAssignViewModel model = new AdminProjectUserAssignViewModel();
            var selected = helper.ListProjects(Id);
            model.project = new MultiSelectList(db.Projects, "Id", "Title", selected);
            model.User = user;

            return View(model);
        }

        //POST: Admin/ProjectUser
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        public ActionResult ProjectUser(AdminProjectUserAssignViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(model.User.Id);
                var currentProjects = db.Projects.Where(p => p.UserId == user.Id).ToList();
                foreach (var current in currentProjects)
                {
                    var project = db.Projects.Find(current);
                    project.Users.Remove(model.User);
                }
                foreach (var selected in model.SelectedProjects)
                {
                    var project = db.Projects.Find(selected);
                    project.Users.Add(model.User);
                }

                db.Entry(user).State = EntityState.Modified;
                db.Users.Attach(user);
                db.SaveChanges();
                    return RedirectToAction("Index", "Admin");
                    
                }
                return View();
            }
        
        
   
        


        //    var user = db.Users.Find(model.User.Id);
        //        //var currentProjects = (from p in db.Projects
        //        //                       where p.Users.Any(r => r.Id == user.Id)
        //        //                       select p.Id).ToArray();
        //        var currentProjects = db.Projects.Where(r => r.UserId == user.Id).ToList();

        //        foreach (var selected in model.SelectedProjects)
        //        {
        //            var project = db.Projects.Find(selected);
        //            project.Users.Add(user);
        //        }
        //        foreach (var current in currentProjects)
        //        {
        //            var project = db.Projects.Find(current);
        //            project.Users.Remove(user);
        //        }
        //        foreach (var selected in model.SelectedProjects)
        //        {
        //            if (selected != current)
        //            {
        //                project.Users.Remove(user);
        //            }
        //        }

        //        db.Entry(user).State = EntityState.Modified;
        //        db.Users.Attach(user);
        //        db.SaveChanges();
        //        return RedirectToAction("Index", "Admin");
        //    }
        //    return View();
        //}
    


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

