using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Models.Domain;
using Trackily.Validation;

namespace Trackily.Models.Binding
{
    public class EditTicketBinding
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public Ticket.TicketType Type { get; set; }

        [Required]
        public Ticket.TicketPriority Priority { get; set; }

        [Required]
        [Display(Name = "Mark as Reviewed")]
        public bool IsReviewed { get; set; }

        [Required]
        [Display(Name = "Mark as Approved")]
        public bool IsApproved { get; set; }

        [Required]
        public Ticket.TicketStatus Status { get; set; }

        public Dictionary<string, bool> RemoveAssigned { get; set; }    // (username, T/F). T = will be removed.

        //[Remote(action: "ValidateAssigned", controller: "TicketsController", ErrorMessage="Test")] 
        // TODO: Need to learn AJAX & Unobtrusive JQuery. For now, just validate upon POSTing.
        [ValidUser]
        public string[] AddAssigned { get; set; }
    }
}
