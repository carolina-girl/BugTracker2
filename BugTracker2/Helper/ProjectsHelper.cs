using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;

namespace BugTracker2.Models.Helper
{
        public class ProjectsHelper
        {
            private ApplicationDbContext db = new ApplicationDbContext();
            private UserManager<ApplicationUser> userManager = new
           UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            public ProjectsHelper(ApplicationDbContext context)
            {
                this.db = context;
            }
            public ProjectsHelper()
            {
            }


        public void AssignedUser(string userId, int projectId)
        {
            if (!HasProject(userId, projectId))
            {
                var user = db.Users.Find(userId);
                var project = db.Projects.Find(projectId);
                project.Users.Add(user);
            }
        }

        public bool HasProject(string userId, int projectId)
        {
            var user = db.Users.Find(userId);
            var project = db.Projects.Find(projectId);
            if (project.Users.Contains(user))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemoveUser(string userId, int projectId)
        {
            if (HasProject(userId, projectId))
            {
                var user = db.Users.Find(userId);
                var project = db.Projects.Find(projectId);
                project.Users.Remove(user);
            }
        }

        public List<int> ListProjects(string userId)
        {
            var user = db.Users.Find(userId);
            return user.Projects.Select(p => p.Id).ToList();
        }

        public List<ApplicationUser> ListUsers(int projectId)
        {
            var project = db.Projects.Find(projectId);
            return project.Users.ToList();
        }
      

        public List<string> ListProjectManagers(int? projectId)
        {
            var projectManagers = new List<string>();
            var project = db.Projects.Find(projectId);
            var projectUsers = project.Users.ToList();
            UserRoleAssignHelper helper = new UserRoleAssignHelper();
            foreach (var user in projectUsers)
            {
                if (helper.IsUserInRole(user.Id, "ProjectManager"))
                {
                    projectManagers.Add(user.Email);
                }
            }
            return projectManagers;
        }
    }

}