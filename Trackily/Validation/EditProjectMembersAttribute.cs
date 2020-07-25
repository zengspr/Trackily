using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Binding.Project;

namespace Trackily.Validation
{
    public class EditProjectMembersAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object usernames, ValidationContext validationContext)
        {
            var context = (TrackilyContext)validationContext.GetService(typeof(TrackilyContext));
            Debug.Assert(context != null);

            if (ValidationHelper.SomeUsersDoNotExist((List<string>)usernames, context))
            {
                return new ValidationResult("One or more assigned users do not exist.");
            }

            var projectToValidate = (ProjectEditBindingModel)validationContext.ObjectInstance;
            var project = context.Projects
                                .Include(p => p.Members)
                                    .ThenInclude(up => up.User)
                                .Single(p => p.ProjectId == projectToValidate.ProjectId);

            if (ValidationHelper.SomeUsersAlreadyMembersOfProject((List<string>)usernames, project))
            {
                return new ValidationResult("One or more users are already members of this Project.");
            }

            return ValidationResult.Success;
        }
    }
}
