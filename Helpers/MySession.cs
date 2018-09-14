using System.Web;

namespace NRAKO_IvanCicek.Helpers
{ 
    public static class MySession
    {
        public static object Get(string key)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session[key] != null)
            {
                return HttpContext.Current.Session[key];
            }
            return null;
        }

        public static void Set(string key, object value)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Session[key] = value;
            }
        }
    }
}