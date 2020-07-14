using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Trackily.Models.View
{
    public class IndexProjectViewModel
    {
        public Guid ProjectId { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Creator")]
        public string CreatorName { get; set; }

        [DisplayName("Created")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:g}")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Tickets")]
        public int NumTickets { get; set; }

        [DisplayName("Members")]
        public int NumMembers { get; set; }
    }
}
