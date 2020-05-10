using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Models.Domain;
using Trackily.Models.Services;
using Trackily.Services.DataAccess;

namespace Trackily.Services.Business
{
    public class UserTicketService 
    {
        public List<string> UserTicketToNames(ICollection<UserTicket> userTickets)
        {
            var usernames = new List<string>();
            foreach (var userTicket in userTickets)
            {
                usernames.Add(userTicket.User.UserName);
            }
            return usernames;
        }
    }
}
