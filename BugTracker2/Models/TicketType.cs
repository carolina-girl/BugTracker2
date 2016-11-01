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
            this.Projects = new HashSet<Projects>();
        }
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Type { get; set; }
        public virtual ICollection<Projects> Projects { get; set; }
    }
}
