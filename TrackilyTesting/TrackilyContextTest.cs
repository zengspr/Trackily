using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Trackily.Areas.Identity.Data;
using Trackily.Controllers;
using Trackily.Models.Domain;
using Xunit;

namespace TrackilyTesting
{
    public class TrackilyContextTest
    {
        [Fact]
        public void DeletingProject_DeletesAssociatedTickets()
        {
            var projectId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();

            var project = new Project
            {
                ProjectId = projectId,
                Tickets = new List<Ticket>()
            };

            project.Tickets.Add(new Ticket
            {
                TicketId = ticketId,
            });

            var options = GetInMemoryDbContextOptions();

            using (var context = new TrackilyContext(options))
            {
                context.Database.EnsureCreated();
                context.Projects.Add(project);
                context.SaveChanges();
            }

            using (var context = new TrackilyContext(options))
            {
                var projectSaved = context.Projects.Include(p => p.Tickets).Single(p => p.ProjectId == projectId);
                Assert.NotNull(projectSaved);

                var ticketSaved = projectSaved.Tickets.ToList()[0];
                Assert.NotNull(ticketSaved);

                context.Projects.Remove(projectSaved);
                context.SaveChanges();
            }

            using (var context = new TrackilyContext(options))
            {
                var projects = context.Projects.ToList();
                Assert.Empty(projects);

                var tickets = context.Tickets.ToList();
                Assert.Empty(tickets);
            }
        }

        [Fact]
        public void DeletingTicket_DeletesUserTicket()
        {
            var ticketId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var options = GetInMemoryDbContextOptions();

            using (var context = new TrackilyContext(options))
            {
                context.Database.EnsureCreated();

                var user = new TrackilyUser()
                {
                    Id = userId,
                    CreatedTickets = new List<Ticket>(),
                    AssignedTo = new List<UserTicket>()
                };

                context.Users.Add(user); // User must exist in the database before creating a Ticket/UserTicket due to FK constraint.
                context.SaveChanges();
            }

            using (var context = new TrackilyContext(options))
            {
                var userSaved = context.Users.Find(userId);
                Assert.NotNull(userSaved);

                var ticket = new Ticket()
                {
                    Project = new Project(),
                    Creator = userSaved,
                    TicketId = ticketId,
                    Assigned = new List<UserTicket>()
                };

                var userTicket = new UserTicket()
                {
                    Id = userId,
                    TicketId = ticketId,
                    Ticket = ticket,
                    User = userSaved
                };

                ticket.Assigned.Add(userTicket);

                context.Tickets.Add(ticket);
                context.SaveChanges();
            }

            using (var context = new TrackilyContext(options))
            {
                var ticketSaved = context.Tickets.Find(ticketId);
                Assert.NotNull(ticketSaved);

                var userTicketSaved = context.UserTickets.Find(userId, ticketId);
                Assert.NotNull(userTicketSaved);

                context.Tickets.Remove(ticketSaved);
                context.SaveChanges();
            }

            using (var context = new TrackilyContext(options))
            {
                var tickets = context.Tickets.ToList();
                Assert.Empty(tickets);

                var userTickets = context.UserTickets.ToList();
                Assert.Empty(userTickets);
            }
        }

        public static DbContextOptions<TrackilyContext> GetInMemoryDbContextOptions()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            return new DbContextOptionsBuilder<TrackilyContext>()
                .UseSqlite(connection)
                .Options;
        }
    }
}
