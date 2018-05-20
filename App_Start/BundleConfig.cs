using System.Web;
using System.Web.Optimization;

namespace NRAKO_IvanCicek
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-3.2.1.js",
                         "~/Scripts/jquery.unobtrusive-ajax.min.js",                      
                        "~/Scripts/jquery.datetimepicker.full.min.js",
                        "~/Scripts/jquery-ui.min.js",
                        "~/Scripts/GlobalJS.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-2.8.3"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/jquery.datetimepicker.css",
                      "~/Content/site.css",
                      "~/Content/jquery-ui.min.css"));

            BundleTable.EnableOptimizations = true;
        }
    }
}
