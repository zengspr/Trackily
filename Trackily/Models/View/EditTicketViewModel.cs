using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Trackily.Models.Binding;
using Trackily.Models.Domain;

namespace Trackily.Models.View
{
    public class EditTicketViewModel : EditTicketBinding
    {

        [Display(Name = "Assigned Developers")]
        public List<string> Assigned { get; set; }

        public List<string> Errors { get; set; }

        public List<CommentThread> CommentThreads { get; set; }
    }
}
