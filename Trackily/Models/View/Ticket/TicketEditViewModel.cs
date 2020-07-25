using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Trackily.Models.View.Ticket
{
    public class TicketEditViewModel : TicketBaseViewModel
    {
        public Guid TicketId { get; set; }

        [DisplayName("Project")]
        public string ProjectTitle { get; set; }

        [DisplayName("Creator")]
        public string CreatorName { get; set; }

        [DisplayName("Created")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:f}")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Last Updated")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:f}")]
        public DateTime UpdatedDate { get; set; }


        public string Content { get; set; }

        public Domain.Ticket.TicketStatus Status { get; set; }

        [DisplayName("Assigned users")]
        public string[] AddAssigned { get; set; }

        [DisplayName("Unassign users")]
        public Dictionary<string, bool> RemoveAssigned { get; set; }

        public List<string> Errors { get; set; }
    }
}
