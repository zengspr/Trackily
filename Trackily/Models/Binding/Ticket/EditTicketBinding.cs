using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Trackily.Validation;

namespace Trackily.Models.Binding.Ticket
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
        public string CreatorName { get; set; }

        [Required]
        public Domain.Ticket.TicketStatus Status { get; set; }

        [EditTicketAssigned]
        public string[] AddAssigned { get; set; }

        [Display(Name = "Unassign Users")]
        public Dictionary<string, bool> RemoveAssigned { get; set; }    // RemoveAssigned[username] = true -> Unassign user.
    }
}
