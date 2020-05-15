using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Domain;
using Trackily.Services.Business;
using Trackily.Services.DataAccess;

namespace Trackily.Models.Services
{
    public class UserService
    {
        // Responsible for providing services related to Users:
        // - Get User object making a request and its properties, such as Claims.
        private readonly UserManager<TrackilyUser> _userManager;
        private readonly DbService _dbService;
        private readonly UserTicketService _userTicketService;

        public UserService(UserManager<TrackilyUser> userManager, DbService dbService, UserTicketService userTicketService)
        {
            _userManager = userManager;
            _dbService = dbService;
            _userTicketService = userTicketService;
        }

        public async Task<TrackilyUser> GetUser(HttpContext request)
        {
            var user = await _userManager.GetUserAsync(request.User);
            return user;
        }

        public async Task<string> GetUserName(HttpContext request)
        {
            var user = await _userManager.GetUserAsync(request.User);
            return user.UserName;
        }

        public async Task<bool> UsernameNotExists(string[] usernames)
        {
            foreach (string username in usernames)
            {
                var exists = await _dbService.GetUser(username);
                if (exists == null)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> UsernameAlreadyAssigned(Guid ticketId, string[] usernames)
        {
            var ticket = await _dbService.GetTicket(ticketId);
            var assignedNames = _userTicketService.UserTicketToNames(ticket.Assigned);
            foreach (string username in usernames)
            {
                if (assignedNames.Contains(username))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
