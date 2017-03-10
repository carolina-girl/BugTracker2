using BugTracker2.Models;
using BugTracker2.Models.Helper;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace BugTracker2.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        UserRoleAssignHelper userManager = new UserRoleAssignHelper();
        ProjectsHelper phelper = new ProjectsHelper();

        // GET: Admin
        [Authorize(Roles = "Admin, ProjectManager")]
        public ActionResult Index()
        {
            List<AdminUserViewModel> userList = new List<AdminUserViewModel>();
            foreach (var user in db.Users.ToList())
            {
                var model = new AdminUserViewModel();
                model.User = user;
                model.role = userManager.ListUserRoles(user.Id).ToList();
                model.projects = phelper.ListProjects(user.Id).ToList();
                userList.Add(model);
            }
            return View(userList);
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
            var selected = phelper.ListProjects(Id);
            model.Projects = new MultiSelectList(db.Projects, "Id", "Title", selected);
            model.User = user;

            return View(model);
        }

        //POST: Admin/ProjectUser
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        public ActionResult ProjectUser(string Id, AdminProjectUserAssignViewModel model)
        {
            var user = db.Users.Find(model.User.Id);
            foreach (var project in user.Projects.ToList())
            {
                if (!model.SelectedProjects.Contains(project.Id))
                {
                    phelper.RemoveProjectFromUser(user.Id, project.Id);
                }
            }
            foreach (var projectId in model.SelectedProjects)
            {
                if (model.SelectedProjects.Contains(projectId))
                {
                    phelper.AddProjectToUser(user.Id, projectId);
                }
            }
        
            db.SaveChanges();
            return RedirectToAction("Index");
        }





        //    var user = db.Users.Find(model.User.Id);
        //    var currentProjects = (from p in db.Projects
        //                           where p.Users.Any(r => r.Id == user.Id)
        //                           select p.Id).ToArray();


        //    foreach (var x in model.SelectedProjects)
        //    {
        //        var project = db.Projects.Find(x);
        //        project.Users.Add(user);
        //    }
        //    foreach (var z in currentProjects)
        //    {
        //        var project = db.Projects.Find(z);

        //        foreach (var x in model.SelectedProjects)
        //        {
        //            if (x != z)
        //            {
        //                project.Users.Remove(user);
        //            }
        //        }
        //    }

        //    db.Entry(user).State = EntityState.Modified;
        //    db.Users.Attach(user);
        //    db.SaveChanges();
        //    return RedirectToAction("Index", "Admin");
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

