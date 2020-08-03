using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Domain;
using Trackily.Services;
using Xunit;

namespace TrackilyTesting
{
    public class UserTicketServiceTest
    {
        private const string MockUsername = "USERNAME";

        [Fact]
        public void UserTicketToNames_ReturnsNamesCorrectly()
        {
            var context = GetDb();

            var userId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();
            SetupUserAndTicket(userId, ticketId, context);

            var user = context.Users.Find(userId);
            var ticket = context.Tickets.Find(ticketId);
            ticket.Assigned.Add(new UserTicket
            {
                Id = userId,
                TicketId = ticketId,
                Ticket = ticket,
                User = user
            });
            context.SaveChanges();

            var ticketSaved = context.Tickets.Include(t => t.Assigned)
                                            .Single(t => t.TicketId == ticketId);
            var service = new UserTicketService(context);
            var usernames = service.UserTicketToNames(ticketSaved.Assigned);

            Assert.Single(usernames, MockUsername);
        }

        [Fact]
        public void UserTicketToNames_NoAssignedUsersReturnsEmpty()
        {
            var context = GetDb();

            var userId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();
            SetupUserAndTicket(userId, ticketId, context);

            var ticketSaved = context.Tickets.Include(t => t.Assigned)
                .Single(t => t.TicketId == ticketId);
            var service = new UserTicketService(context);
            var usernames = service.UserTicketToNames(ticketSaved.Assigned);

            Assert.Empty(usernames);
        }

        private TrackilyContext GetDb()
        {
            var options = TrackilyContextTest.GetInMemoryDbContextOptions();
            var context = new TrackilyContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        private void SetupUserAndTicket(Guid userId, Guid ticketId, TrackilyContext context)
        {
            var user = new TrackilyUser()
            {
                Id = userId,
                UserName = MockUsername,
                CreatedTickets = new List<Ticket>(),
                AssignedTo = new List<UserTicket>()
            };

            context.Users.Add(user);
            context.SaveChanges();

            var userSaved = context.Users.Find(userId);
            var ticket = new Ticket()
            {
                Project = new Project(),
                Creator = userSaved,
                TicketId = ticketId,
                Assigned = new List<UserTicket>()
            };

            context.Tickets.Add(ticket);
            context.SaveChanges();
        }
    }
}
