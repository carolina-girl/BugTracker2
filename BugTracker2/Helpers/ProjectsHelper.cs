using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models.Helper
{
    public class ProjectsHelper
    {
        private ApplicationDbContext db;

        public ProjectsHelper(ApplicationDbContext context)
        {
            this.db = context;
        }

        public void AssignUser(string userId, int projectId)
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

        public List<Projects> ListProjects(string userId)
        {
            var user = db.Users.Find(userId);
            return user.Projects.ToList();
        }

        public List<ApplicationUser> ListUsers(int projectId)
        {
            var project = db.Projects.Find(projectId);
            return project.Users.ToList();
        }

        public List<string> ListProjectManagers(int projectId)
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