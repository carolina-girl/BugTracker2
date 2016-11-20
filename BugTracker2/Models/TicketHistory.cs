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
        public string HistoryUserId { get; set; }
        public string History { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string Title { get; set; }
        public DateTimeOffset Date { get; set; }
        public virtual ICollection<Projects> Projects { get; set; }
        public virtual Tickets Tickets { get; set; }
        public virtual ApplicationUser AssignedUser { get; set; }
    }
}