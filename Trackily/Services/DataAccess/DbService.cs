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

        public async Task<Ticket> GetTicket(Guid? ticketId)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.TicketId == ticketId);
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

        //// Code adapted from https://stackoverflow.com/a/51261289.
        //public void UpdateManyToMany<TDependentEntity, TKey>(IEnumerable<TDependentEntity> dbEntries,
        //                                                     IEnumerable<TDependentEntity> updatedEntries,
        //                                                     Func<TDependentEntity, TKey> keyRetrievalFunction)
        //                                                     where TDependentEntity : class
        //{
        //    var oldItems = dbEntries.ToList();
        //    var newItems = updatedEntries.ToList();
        //    var toBeRemoved = oldItems.LeftComplementRight(newItems, keyRetrievalFunction);
        //    var toBeAdded = newItems.LeftComplementRight(oldItems, keyRetrievalFunction);
        //    var toBeUpdated = oldItems.Intersect(newItems, keyRetrievalFunction);

        //    _context.Set<TDependentEntity>().RemoveRange(toBeRemoved);
        //    _context.Set<TDependentEntity>().AddRange(toBeAdded);
        //    foreach (var entity in toBeUpdated)
        //    {
        //        var changed = newItems.Single(i => keyRetrievalFunction.Invoke(i).Equals(keyRetrievalFunction.Invoke(entity)));
        //        _context.Entry(entity).CurrentValues.SetValues(changed);
        //    }
        //}

        //public void UpdateAssigned<TDependentEntity, TKey>(ICollection<TrackilyUser> existing,
        //                                                   IEnumerable<TDependentEntity> updated,
        //                                                   Func<TDependentEntity, TKey> keyRetrievalFunction)
        //                                                   where TDependentEntity : class
        //{
        //    var oldItems = dbEntries.ToList();
        //    var newItems = updatedEntries.ToList();
        //    var toBeRemoved = oldItems.LeftComplementRight(newItems, keyRetrievalFunction);
        //    var toBeAdded = newItems.LeftComplementRight(oldItems, keyRetrievalFunction);
        //    var toBeUpdated = oldItems.Intersect(newItems, keyRetrievalFunction);

        //    _context.Set<TDependentEntity>().RemoveRange(toBeRemoved);
        //    _context.Set<TDependentEntity>().AddRange(toBeAdded);
        //}
    }
}

