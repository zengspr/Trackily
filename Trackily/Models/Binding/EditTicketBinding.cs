using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Models.Domain;


namespace Trackily.Models.Binding
{
    public class EditTicketBinding : CreateTicketBinding
    {
        public bool IsReviewed { get; set; }    
        public bool IsApproved { get; set; }
        public Ticket.TicketStatus Status { get; set; }
    }
}
