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
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<TrackilyContext>()
                .UseSqlite(connection)
                .Options;

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
            // TODO
        }
    }
}
