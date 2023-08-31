using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceWebApp.Attributes
{
    public class AdminAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("userId");

            if (userId.HasValue && userId.Value == 1)
            {
                // User is an admin, allow access to the action
                base.OnActionExecuting(context);
            }
            else
            {
                // User is not an admin, return a forbidden result
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            }
        }
    }
}