using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Trackily.Models.Domain;

namespace Trackily.Views.TagHelpers
{
    public class TinyMceTagHelper : TagHelper
    {
        private readonly IAuthorizationService _authService;
        private readonly ClaimsPrincipal _principal;

        [HtmlAttributeName("cmt-thread")]
        public CommentThread? CommentThread { get; set; }
        [HtmlAttributeName("cmt")]
        public Comment? Comment { get; set; }

        public TinyMceTagHelper(IAuthorizationService authService, IHttpContextAccessor httpContextAccessor)
        {
            _authService = authService;
            _principal = httpContextAccessor.HttpContext.User;
        }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            // Either a CommentThread or Comment is set as an attribute on the TinyMceTagHelper instance.
            var authResult = CommentThread != null
                ? await _authService.AuthorizeAsync(_principal, CommentThread, "HasEditPrivileges")
                : await _authService.AuthorizeAsync(_principal, Comment, "HasEditPrivileges");

            output.TagName = "textarea";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", authResult.Succeeded ? "TinyMCE" : "TinyMCE-Readonly");
        }
    }
}
