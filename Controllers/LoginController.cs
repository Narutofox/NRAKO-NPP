using NRAKO_IvanCicek.DAL;
using NRAKO_IvanCicek.Factories;
using NRAKO_IvanCicek.Helpers;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace NRAKO_IvanCicek.Controllers
{
    public class LoginController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (MySession.Get("LoginUser") != null && filterContext.ActionDescriptor.ActionName != "Logoff")
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                    { "controller", "Home" },
                    { "action", "Index" }
                });
                return;
            }
        }

        ILoginDAL loginDAL;
        public LoginController()
        {
            loginDAL = DALFactory.GetLoginDAL();
        }

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Login login)
        {
            LoginUser loginUser = loginDAL.LoginCheck(login);
            if (loginUser != null)
            {
                MySession.Set("LoginUser", loginUser);
                return RedirectToAction("Index", "Home");
            }

            TempData["notification"] = new Notification { Text = "Pogrešna email adresa i/ili lozinka", Type = NotificationType.Error };
            return View("Index",login);
        }

        public ActionResult Logoff()
        {
            MySession.Set("LoginUser", null);
            TempData["notification"] = new Notification { Text ="Odjava uspješna", Type = NotificationType.Success  };
            return RedirectToAction("Index");
        }    
    }
}