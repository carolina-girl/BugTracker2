using BugTracker2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static BugTracker2.Models.ApplicationUser;

namespace BugTracker2.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserRoleAssignHelper userRole = new UserRoleAssignHelper();


        // GET: Admin
        public ActionResult Index()
        {
            var model = db.Users.ToList();
            return View(model);
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        //GET: Admin/SelectRole/5
        public ActionResult EditUser(string Id)
        {
            var user = db.Users.Find(Id);
            AdminUserViewModel AdminModel = new AdminUserViewModel();
            var selected = userRole.ListUserRoles(Id);
            AdminModel.Roles = new MultiSelectList(db.Roles, "Name", "Name", selected);
            AdminModel.User = user;

            return View(AdminModel);
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPost]
        public ActionResult EditUser(AdminUserViewModel model)
        {
            var user = db.Users.Find(model.User.Id);
            foreach (var rolermv in db.Roles.Select(r => r.Name).ToList())
            {
                userRole.RemoveUserFromRole(user.Id, rolermv);
            }

            foreach (var roleadd in model.SelectedRoles)
            {
                userRole.AddUserToRole(user.Id, roleadd);
            }
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin,ProjectManager")]
        //GET: Admin/SelectProject/5
        public ActionResult ProjectUser(string Id)
        {
            var user = db.Users.Find(Id);
            AdminProjectUserAssignViewModel AdminProjectModel = new AdminProjectUserAssignViewModel();
            var selected = db.Projects.ToList();
            AdminProjectModel.Projects = new MultiSelectList(db.Projects, "Id", "Title", selected);
            AdminProjectModel.User = user;
            return View(AdminProjectModel);
        }

        [Authorize(Roles = "Admin,ProjectManager")]
        [HttpPost]
        public ActionResult ProjectUser(AdminProjectUserAssignViewModel model)
        {
            var user = db.Users.Find(model.User.Id);
           foreach (var x in model.SelectedProjects)
            {
                var project = db.Projects.Find(x);
                project.Users.Add(user);
            }
            foreach (var x in db.Projects.Select(r => r.Id).ToList())
            {
                var project = db.Projects.Find(x);
                project.Users.Remove(user);
            }
           db.SaveChanges();
            return RedirectToAction("ProjectUser", "Admin");
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
