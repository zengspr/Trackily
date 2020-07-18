using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using Trackily.Areas.Identity.Data;

namespace Trackily.Validation
{
    public class UsersExistAttribute : ValidationAttribute
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

            foreach (string username in usernames.Where(u => u != null))
            {
                // Check whether username exists in the database.
                if (!context.Users.Any(u => u.UserName == username))
                {
                    return new ValidationResult("One or more assigned users do not exist.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
