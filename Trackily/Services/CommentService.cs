using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Binding.Ticket;
using Trackily.Models.Domain;

namespace Trackily.Services
{
    public class CommentService
    {
        private readonly UserManager<TrackilyUser> _userManager;
        private readonly TrackilyContext _context;

        public CommentService(UserManager<TrackilyUser> userManager, TrackilyContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task AddCommentThread(Ticket ticket, TicketDetailsBindingModel input, HttpContext request)
        {

            ticket.CommentThreads.Add(new CommentThread()
            {
                Content = input.CommentThreadContent,
                Parent = ticket,
                Creator = await _userManager.GetUserAsync(request.User)
            });
        }

        public async Task AddComments(TicketDetailsBindingModel input, HttpContext request)
        {
            // Want to avoid adding empty comments, since all comments are POSted.
            foreach (var (replyToThreadId, content) in input.NewReplies.Where(r => r.Value != null))
            {
                var commentThread = _context.CommentThreads.Find(replyToThreadId);

                commentThread.Comments.Add(new Comment
                {
                    Content = content,
                    Parent = commentThread,
                    Creator = await _userManager.GetUserAsync(request.User)
                });
            }
        }
    }
}
