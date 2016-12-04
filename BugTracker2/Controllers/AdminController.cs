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

namespace BugTracker2.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserRoleAssignHelper userRole = new UserRoleAssignHelper();
        AdminUserViewModel AdminModel = new AdminUserViewModel();
        AdminProjectUserAssignViewModel AdminProjectModel = new AdminProjectUserAssignViewModel();



        // GET: Admin 
        [Authorize(Roles = "Admin,ProjectManager")]
        public ActionResult Index()
        {
            var model = db.Users.ToList();
            return View(model);
        }



        //GET: Admin/SelectRole/5
        [Authorize(Roles = "Admin")]
        public ActionResult EditUser(string Id)
        {
            var user = db.Users.Find(Id);
            //AdminUserViewModel AdminModel = new AdminUserViewModel();
            //UserRoleAssignHelper helper = new UserRoleAssignHelper();
            var currentRoles = userRole.ListUserRoles(Id);
            //var absentRoles = userRole.ListAbsentUserRoles(Id);
            //AdminModel.AbsentRoles = new MultiSelectList(absentRoles);
            AdminModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", currentRoles);
            AdminModel.User = user;

            return View(AdminModel);
        }

        // POST: Add User Role
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddRole(string AddId, List<string> SelectedAbsentRoles)
        {
            if (ModelState.IsValid)
            {
                //UserRoleAssignHelper helper = new UserRoleAssignHelper();
                var user = db.Users.Find(AddId);
                foreach (var role in SelectedAbsentRoles)
                {
                    userRole.AddUserToRole(AddId, role);
                }

                db.Entry(user).State = EntityState.Modified;
                db.Users.Attach(user);
                db.SaveChanges();
                return RedirectToAction("ListUsers");
            }
            return View(AddId);
        }

        // POST: Remove User Role
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult RemoveRole(string RemoveId, List<string> SelectedCurrentRoles)
        {
            if (ModelState.IsValid)
            {
                //UserRoleAssignHelper helper = new UserRoleAssignHelper();
                var user = db.Users.Find(RemoveId);
                foreach (var role in SelectedCurrentRoles)
                {
                    userRole.RemoveUserFromRole(RemoveId, role);
                }

                db.Entry(user).State = EntityState.Modified;
                db.Users.Attach(user);
                db.SaveChanges();
                return RedirectToAction("ListUsers");
            }
            return View(RemoveId);
        }




        //GET: Admin/ProjectUser/5
        [Authorize(Roles = "Admin,ProjectManager")]
        public ActionResult ProjectUser(int? Id)
        {
            var user = db.Users.Find(Id);
            AdminProjectUserAssignViewModel AdminProjectModel = new AdminProjectUserAssignViewModel();
            var selected = db.Projects.ToList();
            AdminProjectModel.Projects = new MultiSelectList(db.Projects, "Id", "Title", selected);
            //AdminProjectModel.User = user;
            return View(AdminProjectModel);
        }

        //{
        //    var user = db.Users.Find(Id);
        //    AdminProjectUserAssignViewModel AdminProjectModel = new AdminProjectUserAssignViewModel();
        //    //ProjectId = db.Projects.Id;
        //    //Title = db.Project.Title;
        //    var selected = db.Projects.ToList();
        //    AdminProjectModel.Projects = new MultiSelectList(db.Projects, "Id", "Title", selected);
        //    AdminProjectModel.User = user;
        //    return RedirectToAction("Index", "Admin");
        //}

        // POST: Admin/SelectProject/5
        [HttpPost]
        public ActionResult AssignProjects(string id)
        {
            var user = db.Users.Find(id);
            AdminProjectUserAssignViewModel model = new AdminProjectUserAssignViewModel();
            var selected = db.Projects.ToList();
            model.Projects = new MultiSelectList(db.Projects, "Id", "Title", selected);
            model.User = user;

            return View(model);
        }


        [HttpPost]
        public ActionResult AssignProjects(AdminProjectUserAssignViewModel model)
        {

            var user = db.Users.Find(model.User.Id);
            var currentProjects = (from p in db.Projects
                                   where p.Users.Any(r => r.Id == user.Id)
                                   select p.Id).ToArray();


            foreach (var x in model.SelectedProjects)
            {
                var project = db.Projects.Find(x);
                project.Users.Add(user);
            }
            foreach (var z in currentProjects)
            {
                var project = db.Projects.Find(z);

                foreach (var x in model.SelectedProjects)
                {
                    if (x != z)
                    {
                        project.Users.Remove(user);
                    }
                }
            }

            db.SaveChanges();
            return RedirectToAction("Index", "Admin");
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
