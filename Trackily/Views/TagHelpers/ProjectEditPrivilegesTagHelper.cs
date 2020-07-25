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
    [HtmlTargetElement(Attributes = "edit-project-id")]
    public class ProjectEditPrivilegesTagHelper : TagHelper
    {
        private readonly IAuthorizationService _authService;
        private readonly ClaimsPrincipal _principal;
        public Guid EditProjectId { get; set; }

        public ProjectEditPrivilegesTagHelper(IAuthorizationService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _principal = httpContextAccessor.HttpContext.User;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var authResult = await _authService.AuthorizeAsync(_principal, EditProjectId, "ProjectEditPrivileges");
            if (!authResult.Succeeded)
                output.SuppressOutput();
        }
    }
}
