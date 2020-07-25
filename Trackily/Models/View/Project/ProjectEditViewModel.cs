using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Trackily.Models.View.Project
{
    public class ProjectEditViewModel : ProjectBaseViewModel
    {
        [DisplayName("Created")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Add members")]
        public List<string> AddMembers { get; set; }

        [DisplayName("Remove members")] 
        public Dictionary<string, bool> RemoveMembers { get; set; }

        [DisplayName("Members")]
        public List<Tuple<string, string>> ExistingMembers { get; set; }  // (name, username).

        public List<string> Errors { get; set; }
    }
}
