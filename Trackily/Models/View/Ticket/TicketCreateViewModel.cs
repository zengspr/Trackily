using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;

namespace Trackily.Models.View.Ticket
{
    public class TicketCreateViewModel : TicketBaseViewModel
    {
        [DisplayName("Ticket content")]
        public string Content { get; set; }

        [DisplayName("Assigned users")]
        public string[] AddAssigned { get; set; }

        public string SelectedProject { get; set; }

        [DisplayName("Project")]
        public SelectList Projects { get; set; }

        public List<string> Errors { get; set; }
    }
}
