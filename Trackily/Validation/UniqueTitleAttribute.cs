using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Data;
using Trackily.Models.Binding;

namespace Trackily.Validation
{
    public class UniqueTitleAttribute : ValidationAttribute
    {
        // TODO: Improve the structure of this.
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var context = (TrackilyContext) validationContext.GetService(typeof(TrackilyContext));

            if (validationContext.ObjectType.Name == "EditTicketBinding")
            {
                var input = (EditTicketBinding) validationContext.ObjectInstance;
                if (context.Tickets.Single(t => t.TicketId == input.TicketId).Title == input.Title)
                {
                    return ValidationResult.Success; // Title of the ticket was not changed. 
                }
                if (context.Tickets.Any(t => t.Title == input.Title))
                {
                    return new ValidationResult("Title cannot be identical to another ticket.");
                }
            }
            else if (validationContext.ObjectType.Name == "CreateTicketBinding")
            {
                var input = (CreateTicketBinding)validationContext.ObjectInstance;
                if (context.Tickets.Any(t => t.Title == input.Title))
                {
                    return new ValidationResult("Title cannot be identical to another ticket.");
                }
            }
            else
            {
                throw new Exception("Object type being validated is not known.");
            }
            return ValidationResult.Success;
        }
    }
}
