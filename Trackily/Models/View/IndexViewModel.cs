using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Trackily.Models.Domain.Ticket;

namespace Trackily.Models.View
{
    [MetadataType(typeof(IndexViewModel))]
    public class IndexViewModel 
    {
        public Guid TicketId { get; set; }

        [DisplayName("Creator")]
        public string CreatorUserName { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Priority")]
        public TicketPriority Priority { get; set; }

        [DisplayName("Type")]
        public TicketType Type { get; set; }

        [DisplayName("Status")]
        public TicketStatus Status { get; set; }

        [DisplayName("# Assigned")]
        public int NumAssignedUsers { get; set; }
        
        [DisplayName("Created")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:f}")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Last Updated")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:f}")]
        public DateTime UpdatedDate { get; set; }
    }
}
