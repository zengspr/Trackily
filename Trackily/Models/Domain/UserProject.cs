using System;
using Trackily.Areas.Identity.Data;

namespace Trackily.Models.Domain
{
    // Represents many-to-many relationship between Users and Projects.
    public class UserProject
    {
        public Guid Id { get; set; }
        public TrackilyUser User { get; set; }

        public Guid ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
