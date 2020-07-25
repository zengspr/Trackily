using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;
using Trackily.Areas.Identity.Policies.Requirements;

namespace Trackily.Areas.Identity.Policies.Handlers
{
    public class ProjectEditPrivilegesProjectIdHandler : AuthorizationHandler<ProjectEditPrivilegesRequirement, Guid>
    {
        private readonly TrackilyContext _context;
        private readonly UserManager<TrackilyUser> _userManager;

        public ProjectEditPrivilegesProjectIdHandler(TrackilyContext context, UserManager<TrackilyUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Only the creator of the project or managers who are members of the project can edit the project. 
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ProjectEditPrivilegesRequirement requirement,
            Guid projectId)
        {
            var currentUser = await _userManager.GetUserAsync(context.User);
            Debug.Assert(currentUser != null);

            var project = _context.Projects
                                .Include(p => p.Members)
                                .Single(p => p.ProjectId == projectId);

            if (currentUser.Role == TrackilyUser.UserRole.Manager & project.Members.Any(m => m.Id == currentUser.Id) ||
                currentUser == project.Creator)
            {
                context.Succeed(requirement);
            }
        }
    }
}
