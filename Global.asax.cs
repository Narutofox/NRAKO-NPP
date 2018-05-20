using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace NRAKO_IvanCicek
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
            Response.Headers.Remove("X-Powered-By-Plesk");
        }
        void Application_Error(object sender, EventArgs e)
        {
            Exception exc = Server.GetLastError();
            Server.ClearError();
            Helpers.Logger.Instance.LogException(exc);
            string protocol = "http://";
            string Url = protocol + Request.Url.Authority + "/" + "Error/Index";
            Response.Redirect(Url);
        }
    }
}
