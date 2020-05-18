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
    public class EditTicketBinding : BaseTicketBinding
    {
        [Required]
        [Display(Name = "Mark as Reviewed")]
        public bool IsReviewed { get; set; }

        [Required]
        [Display(Name = "Mark as Approved")]
        public bool IsApproved { get; set; }

        [Required]
        public Ticket.TicketStatus Status { get; set; }

        [ValidUser]
        public new string[] AddAssigned { get; set; }

        [Display(Name = "Unassign Users")]
        public Dictionary<string, bool> RemoveAssigned { get; set; }    // RemoveAssigned[username] = true -> Unassign user.

        // A user can submit content for at most one CommentThread and may make any number of replies to
        // different threads. 
        public string? CommentThreadContent { get; set; } 
        public Dictionary<Guid, string>? CommentContent { get; set; } // Guid specifies CommentThread, Content specifies reply.
    }
}
