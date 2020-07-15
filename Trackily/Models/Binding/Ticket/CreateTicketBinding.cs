using Trackily.Validation;

namespace Trackily.Models.Binding.Ticket
{
	public class CreateTicketBinding : BaseTicketBinding
	{
		[UsersExist]
		public string[] AddAssigned { get; set; }
	}
}
