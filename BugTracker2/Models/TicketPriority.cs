using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models
{
    public class TicketPriority
    {
        public TicketPriority()
        {
            this.Projects = new HashSet<Projects>();
            this.Tickets = new HashSet<Tickets>();
        }
        public int Id { get; set; }
        public int TicketsId { get; set; }
        public string Priority { get; set; }
        public virtual ICollection<Projects> Projects { get; set; }
        public virtual ICollection<Tickets> Tickets { get; set; }

    }
}