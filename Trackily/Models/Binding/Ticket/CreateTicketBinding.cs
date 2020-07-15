using Trackily.Validation;

namespace Trackily.Models.Binding.Ticket
{
	public class CreateTicketBinding : BaseTicketBinding
	{
		[ValidUsernames]
		public string[] AddAssigned { get; set; }
	}
}
