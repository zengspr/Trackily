using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Trackily.Validation;

namespace Trackily.Models.Binding.Ticket
{
    public class TicketEditBindingModel : TicketBaseBindingModel
    {
        public string CreatorName { get; set; }

        // Dates are used to repopulate form if model validation fails, which avoids querying the database.
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        [Required]
        public Domain.Ticket.TicketStatus Status { get; set; }

        [EditTicketAssigned]
        public string[] AddAssigned { get; set; }

        public Dictionary<string, bool> RemoveAssigned { get; set; }    // RemoveAssigned[username] = true -> Unassign user.
    }
}
