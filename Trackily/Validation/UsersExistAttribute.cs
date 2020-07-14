using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Binding;


namespace Trackily.Validation
{
    public class UsersExistAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object addUsers, ValidationContext validationContext)
        {
            var context = (TrackilyContext)validationContext.GetService(typeof(TrackilyContext));
            Debug.Assert(context != null);

            // Check whether username exists in the database.
            foreach (string username in (string[]) addUsers)
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
