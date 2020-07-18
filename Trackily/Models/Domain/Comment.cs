using System;

namespace Trackily.Models.Domain
{
    public class Comment : BaseComment
    {
        public Guid CommentId { get; set; }
        public CommentThread Parent { get; set; }
    }
}
