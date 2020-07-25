using System.ComponentModel;

namespace Trackily.Models.View.Ticket
{
    public class TicketBaseViewModel
    {
        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Type")]
        public Domain.Ticket.TicketType Type { get; set; }

        [DisplayName("Priority")]
        public Domain.Ticket.TicketPriority Priority { get; set; }
    }
}
