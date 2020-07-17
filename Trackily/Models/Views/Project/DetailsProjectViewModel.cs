using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Trackily.Models.Views.Project
{
    public class DetailsProjectViewModel
    {
        public Guid ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        [DisplayName("Created")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Tickets")]
        public List<Domain.Ticket> Tickets { get; set; }

        [DisplayName("Members")]
        public List<Tuple<string, string>> Members { get; set; }  // (name, username).
    }
}
