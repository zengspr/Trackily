using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#nullable enable

namespace Trackily.Models.Domain
{
    public class CommentThread : BaseComment
    {
        public Guid CommentThreadId { get; set; }
        public Ticket Parent { get; set; } // Each CommentThread is associated with one Ticket. 
        public ICollection<Comment>? Comments { get; set; }

        public CommentThread()
        {
            CommentThreadId = new Guid();
        }
    }
}
