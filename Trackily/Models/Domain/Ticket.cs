using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;

namespace Trackily.Models.Domain
{
	/*
     * 		- Guid ticketId
		- ProjectId project
		- String title
		- Datetime created
		- Datetime updated

		- UserId creator
		- List<UserId> assigned
		
		- TicketType type
		- TicketStatus status
		- TicketPriority priority
		- Bool archived
		- Bool reviewed 
		- Bool approved 
		
		- List<CommentThread> threads
		- List<String> files
     */
	public class Ticket : BaseEntity
    {
		public Guid TicketId { get; set; }
		public ICollection<UserTicket> Assigned { get; set; }	// List of Developers assigned to the Ticket.
		public string RelatedFiles { get; set; }	// Name of files related to the Ticket.
		public bool IsReviewed { get; set; }	// Has the Ticket been reviewed by a Manager?
		public bool IsApproved { get; set; }	// Has the Ticket been approved for action by a Manager?

		public enum TicketType { Issue, Feature }
		public enum TicketStatus { Unapproved, Awaiting, Approved }
		public enum TicketPriority { Low, Normal, High }
		public TicketType Type { get; set; }
		public TicketStatus Status { get; set; }
		public TicketPriority Priority { get; set; }
    }
}
