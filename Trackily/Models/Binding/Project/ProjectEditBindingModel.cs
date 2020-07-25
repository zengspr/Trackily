using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Validation;

namespace Trackily.Models.Binding.Project
{
    public class ProjectEditBindingModel : ProjectBaseBindingModel
    {
        [EditProjectMembers]
        public List<string> AddMembers { get; set; }
        public Dictionary<string, bool> RemoveMembers { get; set; }
    }
}
