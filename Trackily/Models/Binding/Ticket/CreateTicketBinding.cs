using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Trackily.Validation;

namespace Trackily.Models.Binding.Ticket
{
    public class CreateTicketBinding : BaseTicketBinding
    {
        [DisplayName("Assign users")]
        [UsersExist]
        public string[] AddAssigned { get; set; }

        [Required]
        public string SelectedProject { get; set; }
    }
}
