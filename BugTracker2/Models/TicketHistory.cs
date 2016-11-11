using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models
{
    public class TicketHistory
    {
        public TicketHistory()
        {
            this.Projects = new HashSet<Projects>();
        }
        public int Id { get; set; }
        public int TicketsId { get; set; }
        public string History { get; set; }
        public virtual ICollection<Projects> Projects { get; set; }
        public virtual Tickets Tickets { get; set; }
    }
}