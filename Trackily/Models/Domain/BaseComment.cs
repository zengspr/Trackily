using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;

namespace Trackily.Models.Domain
{
    public class BaseComment
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public TrackilyUser Creator { get; set; }
        public string Content { get; set; }

        public BaseComment()
        {
            UpdatedDate = DateTime.Now;
            CreatedDate = DateTime.Now;
        }
    }
}
