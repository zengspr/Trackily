using System;
using System.ComponentModel.DataAnnotations;
using Trackily.Areas.Identity.Data;

namespace Trackily.Models.Domain
{
    public class CommentBase
    {
        [Display(Name = "Created")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:f}")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Last Updated")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:f}")]
        public DateTime UpdatedDate { get; set; }

        public TrackilyUser Creator { get; set; }
        public string Content { get; set; }

        public CommentBase()
        {
            UpdatedDate = DateTime.Now;
            CreatedDate = DateTime.Now;
        }
    }
}
