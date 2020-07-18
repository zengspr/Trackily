using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Trackily.Areas.Identity.Policies.Requirements
{
    // Authorization to view details of a Project. 
    public class ProjectEditPrivilegesRequirement : IAuthorizationRequirement
    {
    }
}
