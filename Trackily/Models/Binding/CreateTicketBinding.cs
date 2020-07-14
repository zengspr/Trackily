using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Domain;
using Trackily.Validation;

namespace Trackily.Models.Binding
{
	public class CreateTicketBinding : BaseTicketBinding
	{
		[ValidUsernames]
		public string[] AddAssigned { get; set; }
	}
}
