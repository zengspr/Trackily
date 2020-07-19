using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel;
using Trackily.Models.Binding.Ticket;

namespace Trackily.Models.Views.Ticket
{
    // Derived from CreateTicketBinding to save filled form if an error occurred while submitting.
    public class CreateTicketViewModel : CreateTicketBinding
    {
        [DisplayName("Project")]
        public SelectList Projects { get; set; }
        public List<string> Errors { get; set; }
    }
}
