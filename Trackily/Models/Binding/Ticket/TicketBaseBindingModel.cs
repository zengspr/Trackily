using System;
using System.ComponentModel.DataAnnotations;
using Trackily.Validation;
using static Trackily.Models.Domain.Ticket;

namespace Trackily.Models.Binding.Ticket
{
    public class TicketBaseBindingModel
    {
        public Guid TicketId { get; set; }

        [Required]
        [UniqueTicketTitle]
        [StringLength(60, ErrorMessage = "{0}s must be at least {2} and at most {1} characters long.", MinimumLength = 10)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public TicketType Type { get; set; }

        [Required]
        public TicketPriority Priority { get; set; }
    }
}
