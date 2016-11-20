using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker2.Models
{
    public class AdminUserViewModel
    {
        public ApplicationUser User { get; set; }
        public MultiSelectList Roles { get; set; }
        public string[] SelectedRoles { get; set; }
    }

    public class DashboardViewModel
    {
            public DashboardViewModel()
            {
                this.Projects = new HashSet<Projects>();
                this.Tickets = new HashSet<Tickets>();
            }
        public string Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Tickets> Tickets { get; set; }
        public IEnumerable<Projects> Projects { get; set; }
    }

    public class RolesIndexViewModel
    {
        public IEnumerable<ApplicationUser> Submitters { get; set; }
        public IEnumerable<ApplicationUser> Developers { get; set; }
        public IEnumerable<ApplicationUser> ProjectManagers { get; set; }
        public IEnumerable<ApplicationUser> Administrators { get; set; }
    }
}

