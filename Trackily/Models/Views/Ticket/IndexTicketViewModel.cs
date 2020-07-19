using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static Trackily.Models.Domain.Ticket;

namespace Trackily.Models.Views.Ticket
{
    public class IndexTicketViewModel
    {
        public Guid CreatorId { get; set; }
        public Guid TicketId { get; set; }

        [DisplayName("Creator")]
        public string CreatorName { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Project Title")]
        public string ProjectTitle { get; set; }

        [DisplayName("Priority")]
        public TicketPriority Priority { get; set; }

        [DisplayName("Type")]
        public TicketType Type { get; set; }

        [DisplayName("Status")]
        public TicketStatus Status { get; set; }

        [DisplayName("# Assigned")]
        public int NumAssignedUsers { get; set; }

        [DisplayName("Created")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:g}")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Last Updated")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:g}")]
        public DateTime UpdatedDate { get; set; }
    }
}
