using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using Trackily.Models.Domain;


namespace Trackily.Areas.Identity.Data
{
    public class TrackilyUser : IdentityUser<Guid>
    {
        public enum UserRole { Developer, Manager };
        public UserRole Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public override string UserName { get; set; }

        
        public ICollection<UserProject>? AssignedProjects { get; set; }
        public ICollection<Ticket>? CreatedTickets { get; set; } 
        public ICollection<UserTicket>? AssignedTo { get; set; } 
        public ICollection<CommentThread>? CreatedThreads { get; set; }  
        public ICollection<Comment>? CreatedComments { get; set; }
    }
}
