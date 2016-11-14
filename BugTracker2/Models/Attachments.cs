using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models
{
    public class Attachments
    {
        public int Id { get; set; }
        public int TicketsId { get; set; }
        public string UserId { get; set; }
        public string SubmitterId { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public string MediaUrl { get; set; }
        public DateTimeOffset Created { get; set; }
        public virtual Tickets Tickets { get; set; }
        public virtual ApplicationUser Owner { get; set; }
    }
}