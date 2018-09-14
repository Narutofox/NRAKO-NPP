using System.Web.Mvc;

namespace NRAKO_IvanCicek.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View("Error");
        }
    }
}