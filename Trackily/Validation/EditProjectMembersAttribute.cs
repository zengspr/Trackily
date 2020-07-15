using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Binding.Project;
using Trackily.Models.Domain;

namespace Trackily.Validation
{
    public class EditProjectMembersAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var context = (TrackilyContext)validationContext.GetService(typeof(TrackilyContext));
            Debug.Assert(context != null);


            var usernames = (string[])value;
            if (usernames.All(u => u == null)) // Not adding any users. 
            {
                return ValidationResult.Success;
            }

            var project = (BaseProjectBinding) validationContext.ObjectInstance;
            var loadProject = context.Projects
                .Include(p => p.Members)
                .ThenInclude(up => up.User)
                .Single(p => p.ProjectId == project.ProjectId);

            foreach (string username in usernames.Where(u => u != null))
            {
                // Check whether username exists in the database.
                if (!context.Users.Any(u => u.UserName == username))
                {
                    return new ValidationResult("One or more assigned users do not exist.");
                }

                // Check whether user is already assigned to the ticket.
                var user = context.Users.Single(u => u.UserName == username);
                Debug.Assert(user != null);

                if (loadProject.Members.Any(ut => ut.User.UserName == username))
                {
                    return new ValidationResult("One or more users are already assigned to the Ticket.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
