using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Binding;

namespace Trackily.Validation
{
    public class UniqueProjectTitleAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object projectTitle, ValidationContext validationContext)
        {
            var context = (TrackilyContext)validationContext.GetService(typeof(TrackilyContext));
            Debug.Assert(context != null);


            if (context.Projects.Any(p => p.Title == (string) projectTitle))
            {
                return new ValidationResult("Title cannot be identical to another Project.");
            }
            return ValidationResult.Success;
        }
    }
}
