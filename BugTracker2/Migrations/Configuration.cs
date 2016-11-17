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
                 var userManager = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));
                if (!context.Users.Any(u => u.Email == "mahburns@gmail.com"))
                {
                    userManager.Create(new ApplicationUser
                    {
                        UserName = "mahburns@gmail.com",
                        Email = "mahburns@gmail.com",
                        FirstName = "Mary",
                        LastName = "Burns",
                        DisplayName = "Mary Burns"
                    }, "redhead46");
                }
                 var userId = userManager.FindByEmail("mahburns@gmail.com").Id;
                userManager.AddToRole(userId, "Admin");


                var roleManager2 = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));
                if (!context.Roles.Any(r => r.Name == "ProjectManager"))
                {
                    roleManager.Create(new IdentityRole { Name = "ProjectManager" });
                }
                 var userManager2 = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));
                 if (!context.Users.Any(u => u.Email == "projectManager@coderfoundry.com"))
                {
                    userManager.Create(new ApplicationUser
                    {
                        UserName = "projectManager@coderfoundry.com",
                        Email = "projectManager@coderfoundry.com",
                        FirstName = "ProjectManager",
                        LastName = "ProjectManager",
                        DisplayName = "ProjectManager"
                    }, "Password-1");
                }
                var userId2 = userManager.FindByEmail("projectManager@coderfoundry.com").Id;
                userManager.AddToRole(userId, "ProjectManager");


                var roleManager1 = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));
                if (!context.Roles.Any(r => r.Name == "Developer"))
                {
                    roleManager.Create(new IdentityRole { Name = "Developer" });
                }
                var userManager1 = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));
                if (!context.Users.Any(u => u.Email == "developer@coderfoundry.com"))
                {
                    userManager.Create(new ApplicationUser
                    {
                        UserName = "developer@coderfoundry.com",
                        Email = "developer@coderfoundry.com",
                        FirstName = "Developer",
                        LastName = "Developer",
                        DisplayName = "Developer"
                    }, "Password-1");
                }
                var userId1 = userManager.FindByEmail("developer@coderfoundry.com").Id;
                userManager.AddToRole(userId, "Developer");

                var roleManager3 = new RoleManager<IdentityRole>(
                    new RoleStore<IdentityRole>(context));
                 if (!context.Roles.Any(r => r.Name == "Submitter"))
                {
                    roleManager.Create(new IdentityRole { Name = "Submitter" });
                }
                 var userManager3 = new UserManager<ApplicationUser>(
                    new UserStore<ApplicationUser>(context));
                 if (!context.Users.Any(u => u.Email == "submitter@coderfoundry.com"))
                {
                    userManager.Create(new ApplicationUser
                    {
                        UserName = "submitter@coderfoundry.com",
                        Email = "submitter@coderfoundry.com",
                        FirstName = "Submitter",
                        LastName = "Submitter",
                        DisplayName = "Submitter"
                    }, "Password-1");
                }
                 var userId3 = userManager.FindByEmail("submitter@coderfoundry.com").Id;
                userManager.AddToRole(userId, "Submitter");


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
