using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Domain;

namespace Trackily.Models.Binding
{
	public class CreateTicketBinding 
    {
		public string Title { get; set; }
		// Create a custom attribute to check that each username in the input list exists in the database.
		public string Assigned { get; set; }	// List of Developers assigned to the Ticket.
		public Ticket.TicketType Type { get; set; }
		public Ticket.TicketPriority Priority { get; set; }
    }
}
