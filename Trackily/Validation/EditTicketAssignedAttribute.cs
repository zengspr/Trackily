using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Binding.Ticket;

namespace Trackily.Validation
{
    public class EditTicketAssignedAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var context = (TrackilyContext)validationContext.GetService(typeof(TrackilyContext));
            Debug.Assert(context != null);


            var usernames = (string[])value;
            if (usernames.All(u => u == null)) // Not adding any users. 
            {
                return ValidationResult.Success;
            }

            var ticket = (BaseTicketBinding)validationContext.ObjectInstance;

            var loadTicket = context.Tickets
                .Include(t => t.Assigned)
                .ThenInclude(ut => ut.User)
                .Single(t => t.TicketId == ticket.TicketId);

            foreach (string username in usernames.Where(u => u != null))
            {
                // Check whether username exists in the database.
                if (!context.Users.Any(u => u.UserName == username))
                {
                    return new ValidationResult("One or more assigned users do not exist.");
                }

                // Check whether user is already assigned to the ticket.
                var user = context.Users.Single(u => u.UserName == username);
                Debug.Assert(user != null);

                if (loadTicket.Assigned.Any(ut => ut.User.UserName == username))
                {
                    return new ValidationResult("One or more users are already assigned to the Ticket.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
