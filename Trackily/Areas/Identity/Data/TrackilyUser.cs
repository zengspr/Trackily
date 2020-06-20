using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using Trackily.Models.Domain;


namespace Trackily.Areas.Identity.Data
{
    public class TrackilyUser : IdentityUser<Guid>
    {
        public enum UserRole
        {
            Developer,
            Manager
        };

        public UserRole Role { get; set; }
        public override string UserName { get; set; }

        // Relationships -----------------
        public ICollection<Ticket>? CreatedTickets { get; set; } 
        public ICollection<UserTicket>? AssignedTo { get; set; } 
        public ICollection<CommentThread>? CreatedThreads { get; set; }  
        public ICollection<Comment>? CreatedComments { get; set; }
    }
}
