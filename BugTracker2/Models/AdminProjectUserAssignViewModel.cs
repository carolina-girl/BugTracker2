using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker2.Models
{
    public class AdminProjectUserAssignViewModel
    {
        public ApplicationUser User { get; set; }
        public MultiSelectList Projects { get; set; }
        public MultiSelectList Users { get; set; }
        public Projects Project { get; set; }
        public int[] SelectedProjects { get; set; }
      
    }
}