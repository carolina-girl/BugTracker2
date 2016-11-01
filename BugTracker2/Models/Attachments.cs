using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models
{
    public class Attachments
    {
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Body { get; set; }
    }
}