using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Trackily.Models.Views.Home
{
    public class IndexHomeViewModel
    {
        public string ProjectTitle { get; set; }
        public List<Domain.Ticket> Tickets { get; set; }
    }
}
