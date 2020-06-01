using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Trackily.Models.Binding;
using Trackily.Models.Domain;

namespace Trackily.Models.View
{
    public class EditTicketViewModel : EditTicketBinding
    {
        public List<string> Errors { get; set; }
    }
}
