using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Trackily.Models.Binding;
using Trackily.Models.Domain;
using static Trackily.Models.Domain.Ticket;

namespace Trackily.Models.View
{
    public class DetailsTicketViewModel : DetailsTicketBinding
    {
        // Ticket ~~~~~~~~~~~~~~~~~~~~
        public Guid CreatorId { get; set; }
        public Guid TicketId { get; set; }
        public string Title { get; set; }

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

        [Display(Name = "Assigned Developers")]
        public List<string> Assigned { get; set; }

        public string Content { get; set; }

        public TicketType Type { get; set; }
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }

        // Comments ~~~~~~~~~~~~~~~~~~~~
        public ICollection<CommentThread> CommentThreads { get; set; }
    }
}
