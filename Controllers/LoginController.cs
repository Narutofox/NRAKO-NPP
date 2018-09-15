using NRAKO_IvanCicek.Factories;
using NRAKO_IvanCicek.Helpers;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;
using System.Web.Mvc;
using NRAKO_IvanCicek.Filters;

namespace NRAKO_IvanCicek.Controllers
{
    [LoginControllerFilter]
    public class LoginController : Controller
    {
        private readonly ILoginDAL _loginDal;
        public LoginController()
        {
            _loginDal = DalFactory.GetLoginRepo();
        }

        public LoginController(ILoginDAL loginDal)
        {
            _loginDal = loginDal;
        }

        // GET: Login
        public ViewResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Login login)
        {
            LoginUser loginUser = _loginDal.LoginCheck(login);
            if (loginUser != null)
            {
                MySession.Set("LoginUser", loginUser);
                return RedirectToAction("Index", "Home");
            }

            TempData["notification"] = new Notification
            {
                Text = "Pogrešna email adresa i/ili lozinka",
                Type = NotificationType.Error
            };
            return View("Index",login);
        }

        public ActionResult Logoff()
        {
            MySession.Set("LoginUser", null);
            TempData["notification"] = new Notification
            {
                Text ="Odjava uspješna",
                Type = NotificationType.Success
            };
            return RedirectToAction("Index", "Home");
        }    
    }
}