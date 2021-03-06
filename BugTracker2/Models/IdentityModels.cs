﻿using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using BugTracker2.Models;
using System.Web.Security;
using System.Data;

namespace BugTracker2.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            [NotMapped]
            public string FullName { get { return FirstName + " " + LastName; } }
            public string TimeZone { get; set; }

            public ApplicationUser()
            {
                this.Projects = new HashSet<Projects>();
                this.Tickets = new HashSet<Tickets>();
                this.Comments = new HashSet<Comments>();
                this.Attachments = new HashSet<Attachments>();
            }
            public virtual ICollection<Projects> Projects { get; set; }
            public virtual ICollection<Tickets> Tickets { get; set; }
            public virtual ICollection<Comments> Comments { get; set; }
            public virtual ICollection<Attachments> Attachments { get; set; }

            public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
            {
                // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
                var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
                // Add custom user claims here
                return userIdentity;
            }
        }

        public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
        {
            public ApplicationDbContext()
                    : base("DefaultConnection", throwIfV1Schema: false)
            {
            }

            public static ApplicationDbContext Create()
            {
                return new ApplicationDbContext();
            }

            public DbSet<Comments> Comments { get; set; }

            public DbSet<Attachments> Attachments { get; set; }

            public DbSet<Projects> Projects { get; set; }
            public DbSet<ProjectUsers> ProjectUsers { get; set; }

            public DbSet<TicketHistory> TicketHistory { get; set; }
            public DbSet<TicketNotify> Notifications { get; set; }

        public DbSet<TicketPriority> TicketPriorities { get; set; }

        public DbSet<Tickets> Tickets { get; set; }

        public DbSet<TicketStatus> TicketStatus { get; set; }

        public DbSet<TicketType> TicketTypes { get; set; }

    }
}


