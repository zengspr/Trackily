using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Binding.Ticket;

namespace Trackily.Validation
{
    public class UniqueTicketTitleAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var context = (TrackilyContext)validationContext.GetService(typeof(TrackilyContext));
            Debug.Assert(context != null);

            switch (validationContext.ObjectType.Name)
            {
                case nameof(EditTicketBinding):
                    {
                        var input = (EditTicketBinding)validationContext.ObjectInstance;
                        if (context.Tickets.Single(t => t.TicketId == input.TicketId).Title == input.Title)
                        {
                            return ValidationResult.Success; // Title of the ticket was not changed. 
                        }
                        if (context.Tickets.Any(t => t.Title == input.Title))
                        {
                            return new ValidationResult("Title cannot be identical to another ticket.");
                        }

                        break;
                    }
                case nameof(CreateTicketBinding):
                    {
                        var input = (CreateTicketBinding)validationContext.ObjectInstance;
                        if (context.Tickets.Any(t => t.Title == input.Title))
                        {
                            return new ValidationResult("Title cannot be identical to another ticket.");
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
