using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Domain;

namespace Trackily.Services.DataAccess
{
    public class DbService
    {
        // Responsible for directly performing database queries.

        private readonly TrackilyContext _context;

        public DbService(TrackilyContext context)
        {
            _context = context;
        }

        public async Task<TrackilyUser> GetUserAsync(string username)
        {
            var user = await _context.Users.SingleAsync(u => u.UserName == username);
            return user;
        }

        public async Task<TrackilyUser> GetUserAsync(Guid? userId)
        {
            var user = await _context.Users.SingleAsync(u => u.Id == userId);
            return user;
        }

        public TrackilyUser GetUser(string username)
        {
            return _context.Users.Single(u => u.UserName == username);
        }

        public async Task<Ticket> GetTicket(Guid ticketId)
        {
            var ticket = await _context.Tickets.Include(t => t.Creator)
                                                .Include(t => t.Assigned)   
                                                    .ThenInclude(a => a.User)   
                                                .Include(t => t.CommentThreads)
                                                    .ThenInclude(ct => ct.Comments)
                                                        .ThenInclude(c => c.Creator)
                                                .Include(t => t.CommentThreads)
                                                    .ThenInclude(ct => ct.Creator)
                                                .SingleOrDefaultAsync(t => t.TicketId == ticketId);
            return ticket;
        }

        public async Task<string> GetCreatorName(Ticket ticket)
        {
            var reloadTicket = await _context.Tickets.Include(t => t.Creator)
                .SingleAsync(t => t.TicketId == ticket.TicketId);

            return $"{reloadTicket.Creator.FirstName} {reloadTicket.Creator.LastName}";

        }

        public string GetCreatorName(Project project)
        {
            var reloadProject = _context.Projects.Include(p => p.Creator)
                .Single(p => p.ProjectId == project.ProjectId);

            return $"{reloadProject.Creator.FirstName} {reloadProject.Creator.LastName}";
        }

        public async Task<string> GetUserName(Guid? userId)
        {
            var user = await GetUserAsync(userId);
            return user.UserName;
        }

        public Guid GetKey(TrackilyUser user)
        {
            return user.Id;
        }

        public async Task<Guid> GetKey(string username)
        {
            var user = await GetUserAsync(username);
            return user.Id;
        }
    }
}

