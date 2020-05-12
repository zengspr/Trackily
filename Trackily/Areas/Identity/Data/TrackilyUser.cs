using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Trackily.Models.Domain;

namespace Trackily.Areas.Identity.Data
{
    // TODO: Add authorization to Ticket methods and claims to Users.
    public class TrackilyUser : IdentityUser<Guid>
    {
        public enum UserType
        {
            Developer,
            Manager
        };

        public UserType Type { get; set; }
        public override string UserName { get; set; }

        // Relationships -----------------
        public ICollection<Ticket> CreatedTicket { get; set; } // Each Ticket has one User that is its Creator.
        public ICollection<UserTicket> Assigned { get; set; } // Tickets to which the User has been assigned.
    }
}
