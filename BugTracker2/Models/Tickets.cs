using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Models
{
    public class Tickets
    {
        public Tickets()
        {
            this.Comments = new HashSet<Comments>();
            this.Attachments = new HashSet<Attachments>();
            this.History = new HashSet<TicketHistory>();
        }

        public int Id { get; set; }
        public int OwnerId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset? UpdatedDate { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        public virtual ApplicationUser Project { get; set; }
        public virtual TicketType Type { get; set; }
        public virtual TicketStatus Status { get; set; }
        public virtual TicketPriority Priority { get; set; }
        public virtual ICollection<Comments> Comments { get; set; }
        public virtual ICollection<Attachments> Attachments { get; set; }
        public virtual ICollection<TicketHistory> History { get; set; }

    }
}