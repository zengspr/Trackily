using System;
using System.Collections.Generic;
using Trackily.Validation;

namespace Trackily.Models.Binding.Ticket
{
    [NonEmptyContent]
    public class TicketDetailsBindingModel
    {
        public string NewCommentThreadContent { get; set; } // Content of a new CommentThread.
        public Dictionary<Guid, string> NewComments { get; set; } // Guid specifies CommentThread, Content specifies reply.

        public Dictionary<Guid, string> EditCommentThreads { get; set; }
        public Dictionary<Guid, string> EditComments { get; set; }
        

        public List<string> Errors { get; set; }
    }
}
