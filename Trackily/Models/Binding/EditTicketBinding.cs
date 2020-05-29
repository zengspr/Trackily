using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Trackily.Models.Domain;
using Trackily.Validation;

namespace Trackily.Models.Binding
{
    public class EditTicketBinding : BaseTicketBinding
    {
        [Display(Name = "Created")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:f}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Last Updated")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:f}")]
        public DateTime UpdatedDate { get; set; }

        [Display(Name = "Creator")]
        public string CreatorUserName { get; set; }

        [Required]
        [Display(Name = "Mark as Reviewed")]
        public bool IsReviewed { get; set; }

        [Required]
        [Display(Name = "Mark as Approved")]
        public bool IsApproved { get; set; }

        [Required]
        public Ticket.TicketStatus Status { get; set; }

        [ValidUser]
        public string[] AddAssigned { get; set; }

        [Display(Name = "Unassign Users")]
        public Dictionary<string, bool> RemoveAssigned { get; set; }    // RemoveAssigned[username] = true -> Unassign user.
    }
}
