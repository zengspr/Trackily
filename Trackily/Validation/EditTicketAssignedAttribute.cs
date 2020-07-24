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
            var context = (TrackilyContext) validationContext.GetService( typeof(TrackilyContext) );
            Debug.Assert(context != null);

            var usernames = (string[]) value;

            if (ValidationHelper.SomeUsersDoNotExist(usernames, context))
            {
                return new ValidationResult("One or more assigned users do not exist.");
            }

            var ticketToValidate = (TicketEditBindingModel) validationContext.ObjectInstance;

            // Need to load the ticket from the database because the Assigned.User property is not included by default. 
            var ticket = context.Tickets
                                .Include(t => t.Assigned)
                                    .ThenInclude(ut => ut.User)
                                .Single(t => t.TicketId == ticketToValidate.TicketId);

            if (ValidationHelper.SomeUsersAlreadyAssignedToTicket(usernames, ticket))
            {
                return new ValidationResult("One or more users are already assigned to this Ticket.");
            }

            // If no users are being added, both ValidationHelper methods are false and validation succeeds.
            return ValidationResult.Success;
        }
    }
}
