using System.ComponentModel.DataAnnotations;
using System.Linq;
using Trackily.Models.Binding;


namespace Trackily.Validation
{
    public class NonEmptyContentAttribute : ValidUserAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = (DetailsTicketBinding) validationContext.ObjectInstance;

            if (input.CommentThreadContent == null && 
                (input.NewReplies == null || input.NewReplies.All(r => r.Value == null)))
            {
                return new ValidationResult("Nothing is being submitted.");
            }

            return ValidationResult.Success;
        }
    }
}