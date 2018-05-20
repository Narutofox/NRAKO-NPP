using NRAKO_IvanCicek.Helpers;
using NRAKO_IvanCicek.Models.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NRAKO_IvanCicek.Filters
{
    public class AdminOnlyFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (MySession.Get("LoginUser") == null || (MySession.Get("LoginUser") as LoginUser).UserTypeId != (int)UserType.Admin)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                    { "controller", "Home" },
                    { "action", "Index" }
                });
                return;
            }
        }
    }
}