﻿using BugTracker2.Models;
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
        public int[] SelectedProjects { get; set; }
    }
}
