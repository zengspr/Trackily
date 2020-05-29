using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Models.Domain;
using Trackily.Validation;

namespace Trackily.Models.Binding
{
    [NonEmptyContent]
    public class DetailsTicketBinding
    {
        public string CommentThreadContent { get; set; } // Content of a new CommentThread.
        public Dictionary<Guid, string> NewReplies { get; set; } // Guid specifies CommentThread, Content specifies reply.
        public List<string> Errors { get; set; }
    }
}
