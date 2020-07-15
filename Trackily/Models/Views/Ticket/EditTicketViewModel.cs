using System.Collections.Generic;
using Trackily.Models.Binding.Ticket;

namespace Trackily.Models.Views.Ticket
{
    public class EditTicketViewModel : EditTicketBinding
    {
        public List<string> Errors { get; set; }
    }
}
