using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;
using Trackily.Data;
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

        public async Task<TrackilyUser> GetUser(string username)
        {
            var user = await _context.Users.SingleAsync(u => u.UserName == username);
            return user;
        }

        public async Task<TrackilyUser> GetUser(Guid? userId)
        {
            var user = await _context.Users.SingleAsync(u => u.Id == userId);
            return user;
        }

        public async Task<Ticket> GetTicket(Guid ticketId)
        {
            var ticket = await _context.Tickets
                            .Include(t => t.Creator)
                                .ThenInclude(c => c.FirstName)
                            .Include(t => t.Creator)
                                .ThenInclude(c => c.LastName)
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
            var reloadTicket = await _context.Tickets.Include(t => t.Creator).SingleAsync(t => t.TicketId == ticket.TicketId);
            return $"{reloadTicket.Creator.FirstName} {reloadTicket.Creator.LastName}";

        }

        public async Task<string> GetUserName(Guid? userId)
        {
            var user = await GetUser(userId);
            return user.UserName;
        }

        public Guid GetKey(TrackilyUser user)
        {
            return user.Id;
        }

        public async Task<Guid> GetKey(string username)
        {
            var user = await GetUser(username);
            return user.Id;
        }
    }
}

