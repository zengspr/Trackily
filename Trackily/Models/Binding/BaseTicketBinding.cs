using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Validation;
using static Trackily.Models.Domain.Ticket;

namespace Trackily.Models.Binding
{
	public class BaseTicketBinding
	{
		public Guid TicketId { get; set; } 

		[Required]
		[UniqueTitle]
		public string Title { get; set; }

		[Required]
		public string Content { get; set; }

		[Required]
		public TicketType Type { get; set; }

		[Required]
		public TicketPriority Priority { get; set; }
	}
}
