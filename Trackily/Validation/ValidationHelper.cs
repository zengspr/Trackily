using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Domain;

namespace Trackily.Validation
{
    public static class ValidationHelper
    {
        // Ticket validation helper methods.
        public static bool NotAddingUsers(string[] usernames, TrackilyContext context)
        {
            return (usernames.All(u => u == null));
        }

        public static bool SomeUsersDoNotExist(string[] usernames, TrackilyContext context)
        {
            foreach (string username in usernames.Where(u => u != null))
            {
                if (!context.Users.Any(u => u.UserName == username))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool SomeUsersAlreadyAssignedToTicket(string[] usernames, Ticket ticket)
        {
            foreach (string username in usernames.Where(u => u != null))
            {
                // Check whether user is already assigned to the ticket.
                if (ticket.Assigned.Any(ut => ut.User.UserName == username))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool NotChangingTicketTitle(string title, Guid ticketId, TrackilyContext context)
        {
            // Load ticket from database and check whether its current title matches the POSTed title.
            return context.Tickets.Single(t => t.TicketId == ticketId).Title == title;
        }

        public static bool TicketTitleInUse(string title, TrackilyContext context)
        {
            return context.Tickets.Any(t => t.Title == title);
        }

        // Project validation helper methods.
        public static bool NotChangingProjectTitle(string title, Guid projectId, TrackilyContext context)
        {
            // Load project from database and check whether its current title matches the POSTed title.
            return context.Projects.Single(p => p.ProjectId == projectId).Title == title;
        }

        public static bool ProjectTitleInUse(string title, TrackilyContext context)
        {
            return context.Projects.Any(p => p.Title == title);
        }

        public static bool SomeUsersAlreadyMembersOfProject(string[] usernames, Project project)
        {
            foreach (string username in usernames.Where(u => u != null))
            {
                // Check whether user is already assigned to the ticket.
                if (project.Members.Any(up => up.User.UserName == username))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
