using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;

namespace Trackily.Models.Domain
{
    public class Comment : BaseComment
    {
        public Guid CommentId { get; set; }
        public CommentThread Parent { get; set; }
    }
}
 