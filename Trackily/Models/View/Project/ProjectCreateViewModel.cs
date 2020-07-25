using System;
using System.Collections.Generic;
using System.ComponentModel;
using Trackily.Models.Binding.Project;

namespace Trackily.Models.View.Project
{
    // If creating a project fails, this view model is used to save the user's input.
    public class ProjectCreateViewModel : ProjectBaseViewModel
    {
        [DisplayName("Members")]
        public List<string> AddMembers { get; set; }  

        public List<string> Errors { get; set; }
        public bool Redirected { get; set; }
    }
}
