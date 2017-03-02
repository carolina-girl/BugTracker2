using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BugTracker2.Models
{
    public class Tickets
    {
        public Tickets()
        {
            this.Comments = new HashSet<Comments>();
            this.Attachments = new HashSet<Attachments>();
            this.History = new HashSet<TicketHistory>();
            this.Users = new HashSet<ApplicationUser>();
            this.TicketNotify = new HashSet<TicketNotify>();
        }

        public int Id { get; set; }
        [Display(Name = "Owner")]
        public string userId { get; set; }
        public string OwnerId { get; set; }
        public string AssignedUserId { get; set; }
        public string Email { get; set; }
        public int ProjectId { get; set; }
        [Display(Name = "Type")]
        public int? TypeId { get; set; }
        [Display(Name = "Status")]
        public int? StatusId { get; set; }
        [Display(Name = "Priority")]
        public int? PriorityId { get; set; }

        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }

        public virtual TicketType Type { get; set; }
        public virtual TicketStatus Status { get; set; }
        public virtual TicketPriority Priority { get; set; }
        public virtual ICollection<Comments> Comments { get; set; }
        public virtual ICollection<Attachments> Attachments { get; set; }
        public virtual ICollection<TicketHistory> History { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<TicketNotify> TicketNotify { get; set; }
        public virtual Projects Project { get; set; }
        public virtual ApplicationUser AssignedUser { get; set; }
        public virtual ApplicationUser Owner { get; set; }
    }
}

//{
//    public class tickets
//    {
//        public tickets()
//        {
//            this.Comments = new HashSet<Comments>();
//            this.Attachments = new HashSet<Attachments>();
//            this.History = new HashSet<TicketHistory>();
//            this.Users = new HashSet<ApplicationUser>();
//            this.TicketNotify = new HashSet<TicketNotify>();
//        }

//        public int Id { get; set; }
//        [Display(Name = "Owner")]
//        public string userId { get; set; }
//        public string OwnerId { get; set; }
//        public string AssignedUserId { get; set; }
//        public string Email { get; set; }
//        public int ProjectId { get; set; }
//        [Display(Name = "Type")]
//        public int? TypeId { get; set; }
//        [Display(Name = "Status")]
//        public int? StatusId { get; set; }
//        [Display(Name = "Priority")]
//        public int? PriorityId { get; set; }


//        public DateTimeOffset Created { get; set; }
//        public DateTimeOffset? Updated { get; set; }
//        public string Title { get; set; }
//        public string Body { get; set; }

//        public virtual TicketType Type { get; set; }
//        public virtual TicketStatus Status { get; set; }
//        public virtual TicketPriority Priority { get; set; }
//        public virtual ICollection<Comments> Comments { get; set; }
//        public virtual ICollection<Attachments> Attachments { get; set; }
//        public virtual ICollection<TicketHistory> History { get; set; }
//        public virtual ICollection<ApplicationUser> Users { get; set; }
//        public virtual ICollection<TicketNotify> TicketNotify { get; set; }
//        public virtual project Project { get; set; }
//        public virtual ApplicationUser AssignedUser { get; set; }
//        public virtual ApplicationUser Owner { get; set; }
//    }
//}
