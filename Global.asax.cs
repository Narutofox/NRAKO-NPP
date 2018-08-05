using NRAKO_IvanCicek.Helpers;
using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Metrics;

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

            Metric.Config
                .WithHttpEndpoint("http://localhost:1234/")
                .WithAllCounters();
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
            Logger.Instance.LogException(exc);
            string protocol = "http://";
            string url = protocol + Request.Url.Authority + HardcodedValues.ErrorIndexPath;
            Response.Redirect(url);
        }

        protected void Application_End()
        {
            Metric.Config.Dispose();
        }
    }
}
