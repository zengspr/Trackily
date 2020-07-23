using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Trackily.Models.View.Project
{
    public class ProjectDetailsViewModel : ProjectBaseViewModel
    {
        [DisplayName("Created")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Tickets")]
        public List<Domain.Ticket> Tickets { get; set; }

        [DisplayName("Developers")]
        public List<Tuple<string, string>> Developers { get; set; }  // (name, username).

        [DisplayName("Managers")]
        public List<Tuple<string, string>> Managers { get; set; }
    }
}
