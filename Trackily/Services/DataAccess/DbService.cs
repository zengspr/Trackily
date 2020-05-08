﻿using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
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

        public async Task<Ticket> GetTicket(Guid? ticketId, string include = "none")
        {
            Ticket ticket;
            if (include == "assigned")
            {
                ticket = await _context.Tickets
                    .Include(a => a.Assigned)   // Without Include the Assigned relationships are not loaded by EF Core.
                        .ThenInclude(a => a.User)   // Without ThenInclude the User property of UserTicket is not loaded.
                    .SingleOrDefaultAsync(t => t.TicketId == ticketId);
            }
            else
            {
                ticket = await _context.Tickets
                    .SingleOrDefaultAsync(t => t.TicketId == ticketId);
            }
            return ticket;
        }

        public async Task<string> GetCreatorUserName(Ticket ticket)
        {
            var userTicket = await _context.UserTickets.FirstOrDefaultAsync(ut => ut.TicketId == ticket.TicketId);
            var creator = await GetUser(userTicket.Id);
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
    }
}

