using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker2.Models
{
    public class AssignTicketsViewModel
    {
        public string TicketTitle { get; set; }
        public int TicketId { get; set; }
        public string TicketAssignedTo { get; set; }

        public SelectList UsersList { get; set; }

        public string UserId { get; set; }

    }
}