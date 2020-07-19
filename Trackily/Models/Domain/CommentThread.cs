﻿using System;
using System.Collections.Generic;
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
