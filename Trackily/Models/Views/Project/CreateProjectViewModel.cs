using System.Collections.Generic;
using Trackily.Models.Binding.Project;

namespace Trackily.Models.Views.Project
{
    public class CreateProjectViewModel : BaseProjectBinding
    {
        public List<string> Errors { get; set; }

        public CreateProjectViewModel()
        {
            Errors = new List<string>();
        }
    }
}
