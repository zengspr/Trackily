using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Trackily.Validation;

namespace Trackily.Models.Binding.Ticket
{
    public class TicketCreateBindingModel : TicketBaseBindingModel
    {
        [UsersExist]
        public List<string> AddAssigned { get; set; }

        [Required]
        public string SelectedProject { get; set; }
    }
}
