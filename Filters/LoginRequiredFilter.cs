﻿using NRAKO_IvanCicek.Helpers;
using System.Web.Mvc;
using System.Web.Routing;

namespace NRAKO_IvanCicek.Filters
{
    public class LoginRequiredFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (MySession.Get("LoginUser") == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                    { "controller", "Login" },
                    { "action", "Index" }
                });
            }           
        }
    }
}