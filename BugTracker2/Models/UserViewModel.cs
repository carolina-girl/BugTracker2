﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models
{
    public class UserViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public IList<string> Roles { get; set; }

        public int ProjectCount { get; set; }
        public int TicketsSubmitted { get; set; }
        public int TicketsAssigned { get; set; }
        public int TicketsResolved { get; set; }
       
        public int? ProjectId { get; set; }
    }
}

