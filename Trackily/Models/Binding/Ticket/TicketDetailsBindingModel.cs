using System;
using System.Collections.Generic;
using Trackily.Validation;

namespace Trackily.Models.Binding.Ticket
{
    [NonEmptyContent]
    public class TicketDetailsBindingModel
    {
        public string CommentThreadContent { get; set; } // Content of a new CommentThread.
        public Dictionary<Guid, string> NewReplies { get; set; } // Guid specifies CommentThread, Content specifies reply.
        public List<string> Errors { get; set; }
    }
}
