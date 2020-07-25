using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Binding.Ticket;

namespace Trackily.Validation
{
    public class UniqueTicketTitleAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object ticketTitle, ValidationContext validationContext)
        {
            var context = (TrackilyContext)validationContext.GetService(typeof(TrackilyContext));
            Debug.Assert(context != null);

            if (validationContext.ObjectType.Name == nameof(TicketEditBindingModel))
            {
                var input = (TicketEditBindingModel)validationContext.ObjectInstance;

                if (ValidationHelper.NotChangingTicketTitle((string)ticketTitle, input.TicketId, context))
                {
                    return ValidationResult.Success;
                }
            }

            if (ValidationHelper.TicketTitleInUse((string)ticketTitle, context))
            {
                return new ValidationResult("Title cannot be identical to an existing Ticket's title.");
            }

            return ValidationResult.Success;
        }
    }
}
