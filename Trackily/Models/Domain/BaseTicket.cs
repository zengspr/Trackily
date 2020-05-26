using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;

namespace Trackily.Models.Domain
{
    public class BaseTicket
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public TrackilyUser Creator { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public BaseTicket()
        {
            UpdatedDate = DateTime.Now;
            CreatedDate = DateTime.Now; 
        }
    }
}
