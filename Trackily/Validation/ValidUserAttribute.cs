using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Data;
using Trackily.Models.Binding;

namespace Trackily.Validation
{
    public class ValidUserAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = (EditTicketBinding)validationContext.ObjectInstance;
            var context = (TrackilyContext)validationContext.GetService(typeof(TrackilyContext));
            int countEmpty = 0;

            foreach (string username in input.AddAssigned)
            {
                if (username == null)
                {
                    countEmpty++;
                    if (countEmpty == input.AddAssigned.Length) // Not assigning any users to the ticket.
                    {
                        return ValidationResult.Success;
                    }
                }

                // Check whether username exists in the database.
                if (!context.Users.Any(u => u.UserName == username))
                {
                    return new ValidationResult("One or more assigned users do not exist.");
                }

                // Check whether username is already assigned to the ticket.
                var user = context.Users.Single(u => u.UserName == username);
                if (user == null)
                {
                    return new ValidationResult("User could not be found.");
                }
                if (context.UserTickets.Any(ut => ut.Id == user.Id))
                {
                    return new ValidationResult("One or more users are already assigned to the ticket.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
