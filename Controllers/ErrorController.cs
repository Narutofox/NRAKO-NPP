using System.Web.Mvc;

namespace NRAKO_IvanCicek.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ViewResult Index()
        {
            return View("Error");
        }
    }
}