using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models
{
    public class Comments
    {
        public int Id { get; set; }
        public int TicketsId { get; set; }
        public string UserId { get; set; }
        public string Body { get; set; }
        public string SubmitterId { get; set; }
        public string Submitted { get; set; }
        public string UpdatedReason { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
        public virtual Tickets Tickets { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}