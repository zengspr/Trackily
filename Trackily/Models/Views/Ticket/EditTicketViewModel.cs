using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Trackily.Models.Binding.Ticket;

namespace Trackily.Models.Views.Ticket
{
    public class EditTicketViewModel : EditTicketBinding
    {
        [Display(Name = "Project")]
        public string ProjectTitle { get; set; }
        public List<string> Errors { get; set; }
    }
}
