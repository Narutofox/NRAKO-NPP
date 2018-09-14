using System.Web.Mvc;
using System.Web.Routing;
using NRAKO_IvanCicek.Helpers;

namespace NRAKO_IvanCicek.Filters
{
    public class SignUpControllerFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (MySession.Get("LoginUser") != null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                    { "controller", "Home" },
                    { "action", "Index" }
                });
            }
        }
    }
}