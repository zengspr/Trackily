using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Binding.Project;

namespace Trackily.Validation
{
    public class UniqueProjectTitleAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object projectTitle, ValidationContext validationContext)
        {
            var context = (TrackilyContext)validationContext.GetService(typeof(TrackilyContext));
            Debug.Assert(context != null);

            if (validationContext.ObjectType.Name == nameof(ProjectEditBindingModel))
            {
                // Only in the case when we are editing a project do we need to return a successful validation when the
                // title is not being changed (since all form values are POSTed).
                var input = (ProjectEditBindingModel)validationContext.ObjectInstance;

                if (ValidationHelper.NotChangingProjectTitle(input.Title, input.ProjectId, context))
                {
                    return ValidationResult.Success;
                }
            }

            if (ValidationHelper.ProjectTitleInUse((string)projectTitle, context))
            {
                return new ValidationResult("Title cannot be identical to another existing Project's title.");
            }

            return ValidationResult.Success;
        }
    }
}
