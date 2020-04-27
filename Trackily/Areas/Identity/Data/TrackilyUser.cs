using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Trackily.Models.Domain;

namespace Trackily.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the TrackilyUser class
    public class TrackilyUser : IdentityUser
    {
        public enum UserType
        {
            Developer,
            Manager
        };

        public UserType Type { get; set; }
        public override string UserName { get; set; }
        public ICollection<UserTicket> Assigned { get; set; } // Tickets to which the User has been assigned.
    }
}
