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

        public Ticket Parent { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}
