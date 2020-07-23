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

            var usernames = (string[]) value;

            if (ValidationHelper.SomeUsersDoNotExist(usernames, context))
            {
                return new ValidationResult("One or more assigned users do not exist.");
            }

            // If no users are being added, SomeUsersDoNotExist is false and validation succeeds.
            return ValidationResult.Success;
        }
    }
}
