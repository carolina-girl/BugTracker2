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
        public IEnumerable<Tickets> Tickets { get; set; }
        public IEnumerable<Attachments> Attachments { get; set; }
        public IEnumerable<Comments> Comments { get; set; }
        public int ProjectsAmt { get; set; }
    }
    public class RolesIndexViewModel
    {
        public IEnumerable<ApplicationUser> Submitters { get; set; }
        public IEnumerable<ApplicationUser> Developers { get; set; }
        public IEnumerable<ApplicationUser> ProjectManagers { get; set; }
        public IEnumerable<ApplicationUser> Administrators { get; set; }
    }
}

