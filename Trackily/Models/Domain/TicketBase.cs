using System;
using Trackily.Areas.Identity.Data;

namespace Trackily.Models.Domain
{
    public class TicketBase
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public TrackilyUser Creator { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public TicketBase()
        {
            UpdatedDate = DateTime.Now;
            CreatedDate = DateTime.Now;
        }
    }
}
