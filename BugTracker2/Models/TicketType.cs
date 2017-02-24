using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models
{
    public class TicketType
    {
        public TicketType()
        {
            this.Projects = new HashSet<project>();
            this.Tickets = new HashSet<Tickets>();
        }
        public int Id { get; set; }
        public int TicketsId { get; set; }
        public string Type { get; set; }
        public virtual ICollection<project> Projects { get; set; }
        public virtual ICollection<Tickets> Tickets { get; set; }
    }
}
