using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;

namespace Trackily.Models.Domain
{
    public class BaseEntity
    {
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatorUserName { get; set; }
        public TrackilyUser Creator { get; set; }
        public string Title { get; set; }

        public BaseEntity()
        {
            UpdatedDate = DateTime.Now;
            CreatedDate = CreatedDate ?? UpdatedDate; 
        }
    }
}
