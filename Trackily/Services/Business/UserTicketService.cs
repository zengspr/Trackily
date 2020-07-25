using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Domain;

namespace Trackily.Services.Business
{
    public class UserTicketService
    {
        private readonly TrackilyContext _context;

        public UserTicketService(TrackilyContext context)
        {
            _context = context;
        }

        public UserTicket CreateUserTicket(TrackilyUser user, Ticket ticket)
        {
            return new UserTicket
            {
                Id = user.Id,
                User = user,
                TicketId = ticket.TicketId,
                Ticket = ticket
            };
        }

        public List<string> UserTicketToNames(ICollection<UserTicket> userTickets)
        {
            return userTickets.Select(userTicket => userTicket.User.UserName).ToList();
        }

        public async Task<UserTicket> GetUserTicket(Guid ticketId, Guid userId)
        {
            var userTicket = await _context.UserTickets.FindAsync(userId, ticketId);
            return userTicket;
        }

        // Remove the tickets created by the user from the project and unassign user from all tickets in the project.
        public void RemoveTicketsFromProject(TrackilyUser user, Project project)
        {
            var ticketsCreatedByUser = _context.UserTickets
                                                        .Include(ut => ut.Ticket)
                                                            .ThenInclude(t => t.Creator)
                                                        .Include(ut => ut.Ticket)
                                                            .ThenInclude(t => t.Project)
                                                        .Where(ut => ut.Ticket.Creator.Id == user.Id &
                                                                     ut.Ticket.Project.ProjectId == project.ProjectId)
                                                        .ToList();

            foreach (var userTicket in ticketsCreatedByUser)
            {
                project.Tickets.Remove(userTicket.Ticket);
                _context.UserTickets.Remove(userTicket);
            }

            var projectTicketsUserAssignedTo = _context.UserTickets.Where(ut => ut.User == user).ToList();
            _context.UserTickets.RemoveRange(projectTicketsUserAssignedTo);
        }
    }
}
