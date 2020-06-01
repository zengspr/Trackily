﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Trackily.Areas.Identity.Data;
using Trackily.Areas.Identity.Policies.Requirements;
using Trackily.Models.Domain;

namespace Trackily.Areas.Identity.Policies.Handlers
{
    public class EditPrivilegesUserNameHandler : AuthorizationHandler<EditPrivilegesRequirement, string>
    {
        private readonly UserManager<TrackilyUser> _userManager;

        public EditPrivilegesUserNameHandler(UserManager<TrackilyUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            EditPrivilegesRequirement requirement,
            string resource)
        {
            var user = await _userManager.GetUserAsync(context.User);
            if (user == null)
            {
                return;
            }

            if (user.Role == TrackilyUser.UserRole.Manager || user.UserName == resource)
            {
                context.Succeed(requirement);
            }
        }
    }
}
