using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Models.Binding;
using Trackily.Models.Domain;
using static Trackily.Models.Domain.Ticket;

namespace Trackily.Models.View
{
    // Derived from CreateTicketBinding to save filled form if an error occurred while submitting.
    public class CreateTicketViewModel : CreateTicketBinding
    {
        public List<string> Errors { get; set; }
    }
}
