using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Trackily.Areas.Identity.Data;
using Trackily.Data;
using Trackily.Models.Binding;
using Trackily.Models.Domain;

namespace Trackily.Services.Business
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

        public CommentThread CreateCommentThread(Ticket ticket, string content, TrackilyUser creator)
        {
            return new CommentThread
            {
                Parent = ticket,
                Content = content,
                Creator = creator,
                UpdatedDate = DateTime.Now
            };
        }

        public Comment CreateComment(CommentThread commentThread, string content, TrackilyUser creator)
        {
            return new Comment
            {
                Parent = commentThread,
                Content = content,
                Creator = creator,
                UpdatedDate = DateTime.Now
            };
        }

        public async Task<CommentThread> GetCommentThread(Guid id)
        {
            return await _context.CommentThreads.FindAsync(id);
        }

        public async Task AddCommentThread(Ticket ticket, DetailsTicketBinding input, HttpContext request)
        {
            var commentThread = new CommentThread
            {
                Content = input.CommentThreadContent,
                Parent = ticket,
                Creator = await _userManager.GetUserAsync(request.User)
            };
            ticket.CommentThreads.Add(commentThread);
        }

        public async Task AddComments(Ticket ticket, DetailsTicketBinding input, HttpContext request)
        {
            // Want to avoid adding empty comments, since all comments are POSted.
            foreach (var (replyTo, content) in input.NewReplies.Where(r => r.Value != null))
            {
                var commentThread = await GetCommentThread(replyTo); 
                var comment = new Comment
                {
                    Content = content,
                    Parent = commentThread,
                    Creator = await _userManager.GetUserAsync(request.User),
                };
                commentThread.Comments.Add(comment);
            }
        }
    }
}
