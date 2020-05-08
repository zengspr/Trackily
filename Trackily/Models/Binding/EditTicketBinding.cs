using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Models.Domain;

namespace Trackily.Models.Binding
{
    public class EditTicketBinding 
    {
        public string Title { get; set; }
        public Ticket.TicketType Type { get; set; }
        public Ticket.TicketPriority Priority { get; set; }
        public bool IsReviewed { get; set; }    
        public bool IsApproved { get; set; }
        public Ticket.TicketStatus Status { get; set; }
        public Dictionary<string, bool> RemoveAssigned { get; set; }    // (username, T/F). T = will be removed.

        // [Remote(action: "ValidateAssigned", controller: "Tickets")] - Need to learn AJAX & Unobtrusive JQuery.
        // For now, just validate upon POSTing.
        public string[] AddAssigned { get; set; }
        // TODO: Add custom validation attribute that will ensure i) Username exists within the database AND
        //       ii) Username is not already a user assigned to the Ticket. 
    }
}
