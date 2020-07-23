using System.ComponentModel.DataAnnotations;
using System.Linq;
using Trackily.Models.Binding.Ticket;


namespace Trackily.Validation
{
    public class NonEmptyContentAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = (TicketDetailsBindingModel) validationContext.ObjectInstance;

            // If the user does not click reply, then NewReplies == null. If the user clicks reply but does not include any 
            // text in the reply, the NewReplies != null but the value of each item in NewReplies is null.
            if (input.CommentThreadContent == null && (input.NewReplies == null || input.NewReplies.All(r => r.Value == null)))
            {
                return new ValidationResult("Nothing is being submitted.");
            }

            return ValidationResult.Success;
        }
    }
}