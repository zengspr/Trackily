using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;


namespace Trackily.Controllers.Filters
{
    public class NullIdActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.RouteData.Values["id"] == null)
            {
                context.Result = new NotFoundResult();
            }
        }
    }
}
