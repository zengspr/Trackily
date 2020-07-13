using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;
using Trackily.Areas.Identity.Policies.Requirements;
using Trackily.Models.Domain;


namespace Trackily.Areas.Identity.Policies.Handlers
{
    public class EditPrivilegesTicketHandler : AuthorizationHandler<EditPrivilegesRequirement, Ticket>
    {
        private readonly UserManager<TrackilyUser> _userManager;

        public EditPrivilegesTicketHandler(UserManager<TrackilyUser> userManager)
        {
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            EditPrivilegesRequirement requirement,
            Ticket resource)
        {
            var user = await _userManager.GetUserAsync(context.User);
            if (user == null)
            {
                return;
            }

            if (user.Role == TrackilyUser.UserRole.Manager || resource.Creator == user)
            {
                context.Succeed(requirement);
            }
        }
    }
}
