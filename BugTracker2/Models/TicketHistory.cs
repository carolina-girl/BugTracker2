using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker2.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public DateTimeOffset Date { get; set; }
        public virtual Tickets Ticket { get; set; }
    }
}