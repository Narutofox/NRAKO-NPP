using System.Web.Mvc;
using System.Web.Routing;
using NRAKO_IvanCicek.Helpers;

namespace NRAKO_IvanCicek.Filters
{
    public class LoginControllerFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (MySession.Get("LoginUser") != null && filterContext.ActionDescriptor.ActionName != "Logoff")
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                    { "controller", "Home" },
                    { "action", "Index" }
                });
            }
        }
    }
}