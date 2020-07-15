using System;
using System.Collections.Generic;
using Trackily.Areas.Identity.Data;

namespace Trackily.Models.Domain
{
    public class Project
    {
        public Guid ProjectId { get; set; }
        public TrackilyUser Creator { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public ICollection<Ticket>? Tickets { get; set; }
        public ICollection<UserProject>? Members { get; set; }

        public Project()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
