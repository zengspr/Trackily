using System;
using System.Collections.Generic;


namespace Trackily.Models.Domain
{
	public class Ticket : BaseTicket
	{
		public Guid TicketId { get; set; }
		public enum TicketType { Issue, Feature }
		public enum TicketStatus { Awaiting, Unapproved, Approved, Resolved, Underway, Closed }
		public enum TicketPriority { Normal, Low, High }
		public TicketType Type { get; set; }
		public TicketStatus Status { get; set; }
		public TicketPriority Priority { get; set; }

		public Project Project { get; set; }
		public ICollection<UserTicket>? Assigned { get; set; }	// List of Developers assigned to the Ticket.
		public ICollection<CommentThread>? CommentThreads { get; set; }

		public Ticket()
		{
			Status = TicketStatus.Awaiting;
		}
	}
}
