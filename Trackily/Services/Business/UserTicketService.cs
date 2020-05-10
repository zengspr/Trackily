using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Data;
using Trackily.Models.Domain;
using Trackily.Models.Services;
using Trackily.Services.DataAccess;

namespace Trackily.Services.Business
{
    public class UserTicketService 
    {
        private readonly TrackilyContext _context;

        public UserTicketService(TrackilyContext context)
        {
            _context = context;
        }

        public List<string> UserTicketToNames(ICollection<UserTicket> userTickets)
        {
            var usernames = new List<string>();
            foreach (var userTicket in userTickets)
            {
                usernames.Add(userTicket.User.UserName);
            }
            return usernames;
        }

        public async Task<UserTicket> GetUserTicket(Guid ticketId, Guid userId)
        {
            var userTicket = await _context.UserTickets.FindAsync(userId, ticketId);
            return userTicket;
        }
    }
}
