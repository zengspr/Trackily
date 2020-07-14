using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Binding;

namespace Trackily.Validation
{
    public class ValidUsernamesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var context = (TrackilyContext)validationContext.GetService(typeof(TrackilyContext));
            Debug.Assert(context != null);

            var usernames = (string[]) value;
            if (usernames.All(u => u == null)) // Not assigning any users to the ticket.
            {
                return ValidationResult.Success;
            }

            foreach (string username in usernames.Where(u => u != null))
            {
                // Check whether username exists in the database.
                if (!context.Users.Any(u => u.UserName == username)) 
                {
                    return new ValidationResult("One or more assigned users do not exist.");
                }

                // Check whether username is already assigned to the ticket.
                var user = context.Users.Single(u => u.UserName == username); 
                Debug.Assert(user != null);

                if (context.UserTickets.Any(ut => ut.Id == user.Id))
                {
                    return new ValidationResult("One or more users are already assigned to the Ticket.");
                }
                if (context.UserProjects.Any(up => up.Id == user.Id))
                {
                    return new ValidationResult("One or more users are already assigned to the Project.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
