using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Binding;
using Trackily.Models.Binding.Project;

namespace Trackily.Validation
{
    public class UniqueProjectTitleAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object projectTitle, ValidationContext validationContext)
        {
            var context = (TrackilyContext)validationContext.GetService(typeof(TrackilyContext));
            Debug.Assert(context != null);

            switch (validationContext.ObjectType.Name)
            {
                case nameof(BaseProjectBinding):
                {
                    var input = (BaseProjectBinding) validationContext.ObjectInstance;
                    if (context.Projects.Any(p => p.Title == (string) projectTitle))
                    {
                        return new ValidationResult("Title cannot be identical to another Project's.");
                    }

                    break;
                }
                case nameof(EditProjectBinding):
                {
                    var input = (EditProjectBinding) validationContext.ObjectInstance;
                    if (context.Projects.Single(p => p.ProjectId == input.ProjectId).Title == input.Title)
                    {
                        return ValidationResult.Success;
                    }
                    if (context.Projects.Any(p => p.Title == (string)projectTitle))
                    {
                        return new ValidationResult("Title cannot be identical to another Project's.");
                    }

                    break;
                }
                default:
                    throw new Exception("Object type being validated is not known.");
            }

            return ValidationResult.Success;
        }
    }
}
