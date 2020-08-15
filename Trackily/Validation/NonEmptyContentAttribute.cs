using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Trackily.Models.Binding.Ticket;


namespace Trackily.Validation
{
    public class NonEmptyContentAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var input = (TicketDetailsBindingModel)validationContext.ObjectInstance;
            var httpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            var request = httpContextAccessor.HttpContext.Request;

            if (request.QueryString.ToString() == "?task=create")
            {
                // If the user does not click reply, then NewReplies == null. If the user clicks reply but does not include any 
                // text in the reply, the NewReplies != null but the value of each item in NewReplies is null.
                if (input.NewCommentThreadContent == null && (input.NewComments == null || input.NewComments.All(r => r.Value == null)))
                {
                    return new ValidationResult("Nothing is being submitted.");
                }
            }
            else if (input.EditCommentThreads.Count == 0 && input.EditComments.Count == 0)
            {
                return new ValidationResult("Nothing is being submitted.");
            }

            return ValidationResult.Success;
        }
    }
}