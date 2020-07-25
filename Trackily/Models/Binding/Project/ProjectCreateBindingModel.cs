using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Validation;

namespace Trackily.Models.Binding.Project
{
    public class ProjectCreateBindingModel : ProjectBaseBindingModel
    {
        [UsersExist]
        public List<string> AddMembers { get; set; }
    }
}
