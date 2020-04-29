using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Models.Domain;
using static Trackily.Models.Domain.Ticket;

namespace Trackily.Models.View
{
    public class CreateTicketViewModel
    {
		public List<string> Assigned { get; set; }   // List of Developers assigned to the Ticket.
		public string Title { get; set; }
		public TicketType Type { get; set; }
		public TicketPriority Priority { get; set; }
	}
}
