using System;
using System.Collections.Generic;


namespace Trackily.Models.Domain
{
	// TODO: Remove IsReviewed and IsApproved and all references to it. The semantics of these are already capture in the Status.
	public class Ticket : BaseTicket
	{
		public Guid TicketId { get; set; }
		public ICollection<UserTicket>? Assigned { get; set; }	// List of Developers assigned to the Ticket.
		// public string RelatedFiles { get; set; }	// Name of files related to the Ticket.
		public enum TicketType { Issue, Feature }
		public enum TicketStatus { Awaiting, Unapproved, Approved, Resolved, Closed }
		public enum TicketPriority { Normal, Low, High }
		public TicketType Type { get; set; }
		public TicketStatus Status { get; set; }
		public TicketPriority Priority { get; set; }
		public ICollection<CommentThread>? CommentThreads { get; set; }

		public Ticket()
		{
			Status = TicketStatus.Awaiting;
		}
	}
}
