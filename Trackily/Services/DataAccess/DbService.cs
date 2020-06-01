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
                            .Include(t => t.Assigned)   
                                .ThenInclude(a => a.User)   
                            .Include(t => t.CommentThreads)
                                .ThenInclude(ct => ct.Comments)
                            .SingleOrDefaultAsync(t => t.TicketId == ticketId);
            return ticket;
        }

        // Creator object is still null after Include()
        // Select user id where u.id == creatorId 
        public async Task<string> GetCreatorUserName(Ticket ticket)
        {
            var reloadTicket = await _context.Tickets.Include(t => t.Creator).SingleAsync(t => t.TicketId == ticket.TicketId);
            var creator = reloadTicket.Creator;
            return creator.UserName;
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

