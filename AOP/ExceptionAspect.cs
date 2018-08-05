using PostSharp.Aspects;
using PostSharp.Extensibility;
using System;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using NRAKO_IvanCicek.Helpers;
using PostSharp.Serialization;

namespace NRAKO_IvanCicek.AOP
{
    [PSerializable]
    [MulticastAttributeUsage(MulticastTargets.Method)]
    public class ExceptionAspect : OnExceptionAspect
    {
        public override void OnException(MethodExecutionArgs args)
        {
            var methodInfo = args.Method as MethodInfo;
            if (methodInfo != null)
            {
                Type returnType = methodInfo.ReturnType;
                if (returnType == typeof(int))
                {
                    args.FlowBehavior = FlowBehavior.Return;
                    args.ReturnValue = -1;
                }
                else if (returnType == typeof(bool))
                {
                    args.FlowBehavior = FlowBehavior.Return;
                    args.ReturnValue = false;
                }
                else if (returnType == typeof(ActionResult))
                {
                    args.FlowBehavior = FlowBehavior.Return;
                    if (MySession.Get("LoginUser") != null)
                    {
                        args.ReturnValue = new RedirectToRouteResult(
                            new RouteValueDictionary
                            {
                                {"action", "Index"},
                                {"controller", "Home"}
                            });
                    }
                    else
                    {
                        args.ReturnValue = new RedirectToRouteResult(
                            new RouteValueDictionary
                            {
                                {"action", "Index"},
                                {"controller", "Login"}
                            });
                    }
                   
                }
                else if (returnType.IsClass)
                {
                    args.FlowBehavior = FlowBehavior.Return;
                    args.ReturnValue = null;
                }
            }

            Helpers.Logger.Instance.LogException(args.Exception);
            base.OnException(args);
        }
    }
}