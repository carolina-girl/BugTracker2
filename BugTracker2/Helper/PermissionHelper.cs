using BugTracker2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BugTracker2.Helper
{
    public class PermissionHelper
    {
        private ApplicationDbContext db;

        public PermissionHelper(ApplicationDbContext context)
        {
            this.db = context;
        }

        public bool HasTicketPermission(string userId, int ticketId)
        {
            var user = db.Users.Find(userId);
            var ticket = db.Tickets.Find(ticketId);
            UserRoleAssignHelper helper = new UserRoleAssignHelper(db);
            var userRoles = helper.ListMyRoles(userId);
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
    }
}


