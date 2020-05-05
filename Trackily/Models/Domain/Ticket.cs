using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;

namespace Trackily.Models.Domain
{
	public class Ticket : BaseEntity
    {
		public Guid TicketId { get; set; }
		public ICollection<UserTicket> Assigned { get; set; }	// List of Developers assigned to the Ticket.
		// public string RelatedFiles { get; set; }	// Name of files related to the Ticket.
		public bool IsReviewed { get; set; }	// Has the Ticket been reviewed by a Manager?
		public bool IsApproved { get; set; }	// Has the Ticket been approved for action by a Manager?

		public enum TicketType { Issue, Feature }
		public enum TicketStatus { Awaiting, Unapproved, Approved }
		public enum TicketPriority { Normal, Low, High }
		public TicketType Type { get; set; }
		public TicketStatus Status { get; set; }
		public TicketPriority Priority { get; set; }

		public Ticket()
		{
			TicketId = Guid.NewGuid();
			IsReviewed = false;
			IsApproved = false;
			Status = TicketStatus.Awaiting;
			Assigned = new List<UserTicket>();
		}
    }
}
