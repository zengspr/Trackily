using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trackily.Models.View.Project
{
    public class ProjectBaseViewModel
    {
        public Guid ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
