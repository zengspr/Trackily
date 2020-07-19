using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Trackily.Models.Binding.Project;

namespace Trackily.Models.Views.Project
{
    public class IndexProjectViewModel : BaseProjectBinding
    {
        [DisplayName("Creator")]
        public string CreatorName { get; set; }

        [DisplayName("Created")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Tickets")]
        public int NumTickets { get; set; }

        [DisplayName("Members")]
        public int NumMembers { get; set; }
    }
}
