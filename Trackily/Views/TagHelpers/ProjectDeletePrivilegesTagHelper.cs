using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Trackily.Views.TagHelpers
{
    [HtmlTargetElement(Attributes = "delete-project-id")]
    public class ProjectDeletePrivilegesTagHelper : TagHelper
    {
        private readonly IAuthorizationService _authService;
        private readonly ClaimsPrincipal _principal;
        public Guid DeleteProjectId { get; set; }

        public ProjectDeletePrivilegesTagHelper(IAuthorizationService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _principal = httpContextAccessor.HttpContext.User;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var authResult = await _authService.AuthorizeAsync(_principal, DeleteProjectId, "ProjectDeletePrivileges");
            if (!authResult.Succeeded)
                output.SuppressOutput();
        }
    }
}
