using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models
{
    public class TicketStatus
    {
        public TicketStatus()
        {
            this.Tickets = new HashSet<Tickets>();
            this.Projects = new HashSet<project>();
        }
        public int Id { get; set; }
        public int TicketsId { get; set; }
        public string Status { get; set; }
        public virtual ICollection<Tickets> Tickets { get; set; }
        public virtual ICollection<project> Projects { get; set; }

    }
}
