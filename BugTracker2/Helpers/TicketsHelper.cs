﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace BugTracker2.Models.Helper
{
    public class TicketsHelper
    {
    
        private ApplicationDbContext db = new ApplicationDbContext();

        public TicketsHelper(ApplicationDbContext context)
        {
            this.db = context;
        }

        public List<Tickets> GetUserTickets(string userId)
        {

            var user = db.Users.Find(userId);
            UserRoleAssignHelper userHelper = new UserRoleAssignHelper();
            ProjectsHelper projectHelper = new ProjectsHelper(db);
            var userRoles = userHelper.ListUserRoles(userId);
            var tickets = new List<Tickets>();
            if (userRoles.Contains("Admin"))
            {
                tickets = db.Tickets.Include(t => t.AssignedUser).Include(t => t.Owner).Include(t => t.Project).ToList();
            }
            else if (userRoles.Contains("ProjectManager"))
            {
                tickets = user.Projects.SelectMany(p => p.Tickets).ToList();
            }
            else if (userRoles.Contains("Developer") && userRoles.Contains("Submitter"))
            {
                tickets = db.Tickets.Where(t => t.AssignedUserId == userId || t.OwnerId == userId).Include(t => t.AssignedUser).Include(t => t.Owner).Include(t => t.Project).ToList();

            }
            else if (userRoles.Contains("Developer"))
            {  
                tickets = db.Tickets.Where(t => t.AssignedUserId == userId).Include(t => t.AssignedUser).Include(t => t.Owner).Include(t => t.Project).ToList();
            }
            else if (userRoles.Contains("Submitter"))
            {
                tickets = db.Tickets.Where(t => t.OwnerId == userId).Include(t => t.AssignedUser).Include(t => t.Owner).Include(t => t.Project).ToList();
            }
            return tickets;
        }

        public bool HasTicketPermission(string userId, int ticketId)
        {
            var user = db.Users.Find(userId);
            var ticket = db.Tickets.Find(ticketId);
            UserRoleAssignHelper helper = new UserRoleAssignHelper();
            var userRoles = helper.ListUserRoles(userId);
            if (userRoles.Contains("Admin"))
            {
                return true;
            }
            else if (userRoles.Contains("ProjectManager") && user.Projects.SelectMany(p => p.Tickets).ToList().Contains(ticket))
            {
                return true;
            }
            else if (userRoles.Contains("Submitter") && userRoles.Contains("Developer"))
            {
                if (ticket.AssignedUserId == userId || ticket.OwnerId == userId)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (userRoles.Contains("Developer") && ticket.AssignedUserId == userId)
            {
                return true;
            }
            else if (userRoles.Contains("Submitter") && ticket.OwnerId == userId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void CreateHist(int? newTicketId, string property, string oldValue, string newValue);
        
            var history = new TicketHistory();
            history.TicketId = newTicketId;
            history.Property = property;
            history.OldValue = oldValue;
            history.NewValue = newValue;
            history.Changed = DateTimeOffset.UtcNow;
            history.UserId = User.Identity.GetUserId();
            db.Entry(History).State = EntityState.Added;
            db.SaveChanges();
            return;




        }
    }
