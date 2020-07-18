using System;
using Trackily.Areas.Identity.Data;

namespace Trackily.Models.Domain
{
    public class UserTicket // linking table to model many-to-many relationship b/w User : Ticket.
    {
        public Guid Id { get; set; }
        public TrackilyUser User { get; set; }


        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }
}
