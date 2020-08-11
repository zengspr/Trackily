using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Security.Claims;
using System.Threading.Tasks;
using Trackily.Models.Domain;

namespace Trackily.Views.TagHelpers
{
    /*
     * This tag helper generates a TinyMce textarea that is editable if the principal has editing
     * privileges (either user type: Manager or is Creator of the content) and is readonly otherwise.
     */
    [HtmlTargetElement("tiny-mce")]
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
            var authResult = CommentThread != null ? 
                await _authService.AuthorizeAsync(_principal, CommentThread, "TicketEditPrivileges") 
                : await _authService.AuthorizeAsync(_principal, Comment, "TicketEditPrivileges");

            output.TagName = "textarea";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("class", authResult.Succeeded ? "TinyMCE" : "TinyMCE-Readonly");
        }
    }
}
