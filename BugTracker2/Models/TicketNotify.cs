using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models
{
    public class TicketNotify
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string NotifyUserId { get; set; }

        public virtual ApplicationUser NotifyUser { get; set; }
        public virtual Tickets Ticket { get; set; }
    }
}