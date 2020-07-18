using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Trackily.Areas.Identity.Data;
using Trackily.Areas.Identity.Policies.Requirements;

namespace Trackily.Areas.Identity.Policies.Handlers
{
    public class ProjectDeletePrivilegesProjectIdHandler : AuthorizationHandler<ProjectDeletePrivilegesRequirement, Guid>
    {
        private readonly TrackilyContext _context;
        private readonly UserManager<TrackilyUser> _userManager;

        public ProjectDeletePrivilegesProjectIdHandler(TrackilyContext context, UserManager<TrackilyUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ProjectDeletePrivilegesRequirement requirement,
            Guid projectId)
        {
            var project = _context.Projects.Include(p => p.Creator).Single();
            var currentUser = await _userManager.GetUserAsync(context.User);
            Debug.Assert(currentUser != null);

            if (currentUser.Id == project.Creator.Id)
            {
                context.Succeed(requirement);
            }
        }
    }
}
