using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Trackily.Models.Binding.Ticket;
using Trackily.Models.Domain;
using static Trackily.Models.Domain.Ticket;

namespace Trackily.Models.View.Ticket
{
    public class TicketDetailsViewModel : TicketBaseViewModel
    {
        public Guid CreatorId { get; set; }
        public Guid TicketId { get; set; }

        [Display(Name = "Project")]
        public string ProjectTitle { get; set; }

        [Display(Name = "Creator")]
        public string CreatorName { get; set; }

        [Display(Name = "Created")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:f}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Last Updated")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:f}")]
        public DateTime UpdatedDate { get; set; }

        [Display(Name = "Assigned Developers")]
        public List<string> Assigned { get; set; }

        public string Content { get; set; }
        public TicketStatus Status { get; set; }
        
        public ICollection<CommentThread> CommentThreads { get; set; }
        public List<string> Errors { get; set; }
    }
}
