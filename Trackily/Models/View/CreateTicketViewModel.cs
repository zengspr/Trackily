using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Models.Binding;
using Trackily.Models.Domain;
using static Trackily.Models.Domain.Ticket;

namespace Trackily.Models.View
{
    public class CreateTicketViewModel : CreateTicketBinding
    {
        public List<string> Errors { get; set; }
    }
}
