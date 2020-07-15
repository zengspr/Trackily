using System.Collections.Generic;
using Trackily.Models.Binding.Ticket;

namespace Trackily.Models.Views.Ticket
{
    // Derived from CreateTicketBinding to save filled form if an error occurred while submitting.
    public class CreateTicketViewModel : CreateTicketBinding
    {
        public List<string> Errors { get; set; }
    }
}
