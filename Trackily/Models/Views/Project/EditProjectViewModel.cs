using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Models.Binding.Project;

namespace Trackily.Models.Views.Project
{
    public class EditProjectViewModel : EditProjectBinding
    {
        [DisplayName("Created")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Members")]
        public List<Tuple<string, string>> Members { get; set; }  // (name, username).
    }
}
