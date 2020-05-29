using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Models.Binding;

namespace Trackily.Validation
{
    public class NonEmptyContentAttribute : ValidUserAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = (DetailsTicketBinding) validationContext.ObjectInstance;

            if (input.CommentThreadContent == null && 
                input.NewReplies.All(r => r.Value == null))
            {
                return new ValidationResult("The content of the thread or reply being made cannot be empty.");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }
}
