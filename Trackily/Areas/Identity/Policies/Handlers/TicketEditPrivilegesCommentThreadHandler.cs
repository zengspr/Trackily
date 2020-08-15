using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Trackily.Areas.Identity.Data;
using Trackily.Areas.Identity.Policies.Requirements;
using Trackily.Models.Domain;

namespace Trackily.Areas.Identity.Policies.Handlers
{
    public class TicketEditPrivilegesCommentThreadHandler : AuthorizationHandler<TicketEditPrivilegesRequirement, CommentThread>
    {
        private readonly UserManager<TrackilyUser> _userManager;

        public TicketEditPrivilegesCommentThreadHandler(UserManager<TrackilyUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            TicketEditPrivilegesRequirement requirement,
            CommentThread commentThread)
        {
            var currentUser = await _userManager.GetUserAsync(context.User);
            Debug.Assert(currentUser != null);

            if (currentUser.Id == commentThread.Creator.Id)
            {
                context.Succeed(requirement);
            }
        }
    }
}
