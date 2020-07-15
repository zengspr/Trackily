using System;
using System.ComponentModel.DataAnnotations;
using Trackily.Validation;

namespace Trackily.Models.Binding.Project
{
    public class BaseProjectBinding
    {
        public Guid ProjectId { get; set; }

        [Required]
        [UniqueProjectTitle]
        [StringLength(60, ErrorMessage = "Title must be at least {2} and at most {1} characters long.", MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        [StringLength(125, ErrorMessage = "Project descriptions must be less than {1} characters long.")]
        public string Description { get; set; }

        [UsersExist]
        public string[] AddMembers { get; set; }
    }
}
