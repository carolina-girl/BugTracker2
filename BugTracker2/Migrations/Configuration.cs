using BugTracker2.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;


    namespace BugTracker2.Migrations
    {

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }
        protected override void Seed(ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "ProjectManager"))
            {
                roleManager.Create(new IdentityRole { Name = "ProjectManager" });
            }
            if (!context.Roles.Any(r => r.Name == "Developer"))
            {
                roleManager.Create(new IdentityRole { Name = "Developer" });
            }
            if (!context.Roles.Any(r => r.Name == "Submitter"))
            {
                roleManager.Create(new IdentityRole { Name = "Submitter" });
            }

            var userManager = new UserManager<Models.ApplicationUser>(
                new UserStore<Models.ApplicationUser>(context));
            if (!context.Users.Any(u => u.Email == "AdminEmail"))
            {
                userManager.Create(new Models.ApplicationUser
                {
                    UserName = "mahburns@gmail.com",
                    Email = "mahburns@gmail.com",
                    FirstName = "Mary",
                    LastName = "Burns"
                }, "redhead46");
            }
            if (!context.Users.Any(u => u.Email == "ProjectManagerEmail"))
            {
                userManager.Create(new Models.ApplicationUser
                {
                    UserName = "projectManager@coderfoundry.com",
                    Email = "projectManager@coderfoundry.com",
                    FirstName = "Project Manager"
                }, "Password-1");
            }
            if (!context.Users.Any(u => u.Email == "DeveloperEmail"))
            {
                userManager.Create(new Models.ApplicationUser
                {
                    UserName = "developer@coderfoundry.com",
                    Email = "developer@coderfoundry.com",
                    FirstName = "Developer"
                }, "Password-1");
            }
            if (!context.Users.Any(u => u.Email == "SubmitterEmail"))
            {
                userManager.Create(new Models.ApplicationUser
                {
                    UserName = "submitter@coderfoundry.com",
                    Email = "submitter@coderfoundry.com",
                    FirstName = "Submitter"
                }, "Password-1");
            }
        
            var userId = userManager.FindByEmail("mahburns@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");
            var userId2 = userManager.FindByEmail("projectManager@coderfoundry.com").Id;
            userManager.AddToRole(userId2, "ProjectManager");
            var userId3 = userManager.FindByEmail("developer@coderfoundry.com").Id;
            userManager.AddToRole(userId3, "Developer");
            var userId4 = userManager.FindByEmail("submitter@coderfoundry.com").Id;
            userManager.AddToRole(userId4, "Submitter");


            context.TicketStatus.AddOrUpdate(s => s.Status,
                 new TicketStatus() { Status = "Open" },
                 new TicketStatus() { Status = "Pending" },
                 new TicketStatus() { Status = "Closed" }
                 );

                context.TicketPriorities.AddOrUpdate(p => p.Priority,
                new TicketPriority() { Priority = "High" },

                new TicketPriority() { Priority = "Medium" },

                new TicketPriority() { Priority = "Low" }
                );

                context.TicketTypes.AddOrUpdate(t => t.Type,
                new TicketType() { Type = "Bug" },
                new TicketType() { Type = "Feature Request" },
                new TicketType() { Type = "Documentation" }
                 );
        }
     }
 }





//  This method will be called after migrating to the latest version.

//  You can use the DbSet<T>.AddOrUpdate() helper extension method 
//  to avoid creating duplicate seed data. E.g.
//
//    context.People.AddOrUpdate(
//      p => p.FullName,
//      new Person { FullName = "Andrew Peters" },
//      new Person { FullName = "Brice Lambson" },
//      new Person { FullName = "Rowan Miller" }
//    );
//
