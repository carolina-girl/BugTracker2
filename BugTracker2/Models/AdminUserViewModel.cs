using BugTracker2.Models;
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
        public string Id { get; set; }
        public string Name { get; set; }
 
        public List<Projects> Projects { get; set; }
        public List<Tickets> Tickets { get; set; }
    }

}

