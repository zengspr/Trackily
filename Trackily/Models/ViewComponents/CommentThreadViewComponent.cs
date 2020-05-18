using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Data;
using Trackily.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Trackily.Models.Domain;

namespace Trackily.Models.ViewComponents
{
    /*
    - Edit view calls CommentThread view component with ticket id.
    - CommentThread view component renders the CommentThread & Comment content of the given ticket.
    - User may create a new CommentThread or new Comment replying to an existing thread and POST it 
      as part of the Edit view.
    - How do I specify the model binding from within the view component? It could be called from either the
      Edit or Details views (and possibly others in the future).

     */

    [ViewComponent(Name = "CommentThread")]
    public class CommentThreadViewComponent : ViewComponent
    {
        private readonly TrackilyContext _context;
        public CommentThreadViewComponent(TrackilyContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid ticketId, string parentView)
        {
            // Check behavior when there are no CommentThreads on the given ticket. 
            ViewData["ParentView"] = parentView;
            var commentThreads = await GetCommentThreadsAsync(ticketId);
            return View(commentThreads);
        }

        private async Task<List<CommentThread>> GetCommentThreadsAsync(Guid ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                return null;
            }
            return await _context.CommentThreads
                            .Include(ct => ct.Comments)
                            .Where(ct => ct.Parent == ticket)
                            .ToListAsync();
        }
    }
}
