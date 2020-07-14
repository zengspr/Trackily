using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Trackily.Models.Binding;

namespace Trackily.Models.View
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
