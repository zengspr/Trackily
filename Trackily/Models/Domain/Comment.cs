using System;

namespace Trackily.Models.Domain
{
    public class Comment : CommentBase
    {
        public Guid CommentId { get; set; }
        public CommentThread Parent { get; set; }
    }
}
