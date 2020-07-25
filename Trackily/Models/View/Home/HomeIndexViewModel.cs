using System.Collections.Generic;

namespace Trackily.Models.View.Home
{
    public class HomeIndexViewModel
    {
        public string ProjectTitle { get; set; }
        public List<Domain.Ticket> Tickets { get; set; }
    }
}
