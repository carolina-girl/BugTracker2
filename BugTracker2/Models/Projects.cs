using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BugTracker2.Models
{
    public class Projects
        {
            public Projects()
            {
                this.Users = new HashSet<ApplicationUser>();
                this.Tickets = new HashSet<Tickets>();
            }
            public int Id { get; set; }
            public string UserId { get; set; }
            public string Title { get; set; }
            public DateTimeOffset Created { get; set; }
            public DateTimeOffset? Updated { get; set; }
            public string Body { get; set; }
            public string Slug { get; set; }
            public virtual ICollection<ApplicationUser> Users { get; set; }
            public virtual ICollection<Tickets> Tickets { get; set; }
        }
    }


//    {
//        public project()
//        {
//            this.Users = new HashSet<ApplicationUser>();
//            this.Tickets = new HashSet<tickets>();
//        }
//        public int Id { get; set; }
//        public ApplicationUser user { get; set; }
//        public string userId { get; set; }
//        public string Title { get; set; }
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
//        public DateTimeOffset Created { get; set; }
//        [DataType(DataType.Date)]
//        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
//        public DateTimeOffset? Updated { get; set; }
//        public string Body { get; set; }
//        public string Slug { get; set; }
//        public virtual ICollection<ApplicationUser> Users { get; set; }
//        public virtual ICollection<tickets> Tickets { get; set; }
//    }
//}