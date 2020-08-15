using System;
using System.Diagnostics;
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
                Content = input.NewCommentThreadContent,
                Parent = ticket,
                Creator = await _userManager.GetUserAsync(request.User)
            });
        }

        public void EditCommentThread(TicketDetailsBindingModel input)
        {
            foreach ((Guid key, string content) in input.EditCommentThreads)
            {
                var commentThread = _context.CommentThreads.Find(key);
                Debug.Assert(commentThread != null);
                commentThread.Content = content;
            }
        }

        public async Task AddComments(TicketDetailsBindingModel input, HttpContext request)
        {
            // Want to avoid adding empty comments, since all comments are POSted.
            foreach (var (replyToThreadId, content) in input.NewComments.Where(r => r.Value != null))
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

        public void EditComments(TicketDetailsBindingModel input)
        {
            foreach ((Guid key, string content) in input.EditComments)
            {
                var comment = _context.Comments.Find(key);
                Debug.Assert(comment != null);
                comment.Content = content;
            }
        }
    }
}
