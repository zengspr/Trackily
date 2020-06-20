using System.ComponentModel.DataAnnotations;
using System.Linq;
using Trackily.Data;
using Trackily.Models.Binding;


namespace Trackily.Validation
{
    public class UserExistsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = (CreateTicketBinding)validationContext.ObjectInstance;
            var context = (TrackilyContext)validationContext.GetService(typeof(TrackilyContext));

            // Check whether username exists in the database.
            foreach (string username in input.AddAssigned)
            {
                if (username != null && !context.Users.Any(u => u.UserName == username))
                {
                    return new ValidationResult("One or more assigned users do not exist.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
