using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Trackily.Models.Domain.Ticket;

namespace Trackily.Models.View
{
    public class IndexViewModel 
    {
        public Guid TicketId { get; set; }
        public string Title { get; set; }
        public TicketPriority Priority { get; set; }
        public TicketType Type { get; set; }
        public TicketStatus Status { get; set; }

        [DisplayName("# Assigned")]
        public int NumAssignedUsers { get; set; }
        
        [DisplayName("Created")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:f}")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Last Updated")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:f}")]
        public DateTime UpdatedDate { get; set; }

        [DisplayName("Reviewed")]
        public bool IsReviewed { get; set; }

        [DisplayName("Approved")]
        public bool IsApproved { get; set; }
    }
}
