using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Trackily.Models.Domain;
#nullable enable

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
        public ICollection<Ticket>? CreatedTickets { get; set; } 
        public ICollection<UserTicket>? AssignedTo { get; set; } 
        public ICollection<CommentThread>? CreatedThreads { get; set; }  
        public ICollection<Comment>? CreatedComments { get; set; }
    }
}
