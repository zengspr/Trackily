using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


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
