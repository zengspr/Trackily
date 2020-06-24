using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;
using Trackily.Areas.Identity.Policies.Requirements;


namespace Trackily.Areas.Identity.Policies.Handlers
{
    public class EditPrivilegesUserIdHandler : AuthorizationHandler<EditPrivilegesRequirement, Guid>
    {
        private readonly UserManager<TrackilyUser> _userManager;

        public EditPrivilegesUserIdHandler(UserManager<TrackilyUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            EditPrivilegesRequirement requirement,
            Guid creatorId)
        {
            var currentUser = await _userManager.GetUserAsync(context.User);
            if (currentUser == null)
            {
                return;
            }

            if (currentUser.Role == TrackilyUser.UserRole.Manager || currentUser.Id == creatorId)
            {
                context.Succeed(requirement);
            }
        }
    }
}
