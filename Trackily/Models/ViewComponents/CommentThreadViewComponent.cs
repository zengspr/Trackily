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
    [ViewComponent(Name = "CommentThread")]
    public class CommentThreadViewComponent : ViewComponent
    {
        private readonly TrackilyContext _context;
        public CommentThreadViewComponent(TrackilyContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync(Guid ticketId)
        {
            var commentThreads = await GetCommentThreadsAsync(ticketId);
            return View(commentThreads);
        }

        private async Task<List<CommentThread>> GetCommentThreadsAsync(Guid ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            return await _context.CommentThreads
                            .Include(ct => ct.Comments)
                            .Where(ct => ct.Parent == ticket)
                            .ToListAsync();
        }
    }
}
