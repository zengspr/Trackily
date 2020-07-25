using System.Collections.Generic;
using Trackily.Validation;

namespace Trackily.Models.Binding.Project
{
    public class ProjectCreateBindingModel : ProjectBaseBindingModel
    {
        [UsersExist]
        public List<string> AddMembers { get; set; }
    }
}
