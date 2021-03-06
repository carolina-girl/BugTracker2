﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models
{
    public class ProjectUsers
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectUserId { get; set; }

        public virtual Projects Project { get; set; }
        public virtual ApplicationUser ProjectUser { get; set; }

    }
}