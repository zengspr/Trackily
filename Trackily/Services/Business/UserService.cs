using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Domain;
using Trackily.Services.DataAccess;

namespace Trackily.Models.Services
{
    public class UserService
    {
        // Responsible for providing services related to Users:
        // - Get User object making a request and its properties, such as Claims.
        private readonly UserManager<TrackilyUser> _userManager;
        private readonly DbService _dbService;

        public UserService(UserManager<TrackilyUser> userManager, DbService dbService)
        {
            _userManager = userManager;
            _dbService = dbService;
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
    }
}
