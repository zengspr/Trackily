using Microsoft.AspNetCore.Authorization;

namespace Trackily.Areas.Identity.Policies.Requirements
{
    // Authorization to view details of a Project. 
    public class ProjectEditPrivilegesRequirement : IAuthorizationRequirement
    {
    }
}
