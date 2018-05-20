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
    public class SignUpController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (MySession.Get("LoginUser") != null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary {
                    { "controller", "Home" },
                    { "action", "Index" }
                });
                return;
            }
        }

        IUserDAL userDAL;
        public SignUpController()
        {
            userDAL = DALFactory.GetUserDAL();
        }
        // GET: SignUp
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                user.Salt = Hashing.GetSalt();
                user.Password = Hashing.Hash(user.Password, user.Salt);
                user.RecordStatusId = (int)RecordStatus.Active;
                user.UserTypeId = (int)UserType.Normal;
                userDAL.Create(user);
                MySession.Set("LoginUser", new LoginUser { FirstName = user.FirstName, LastName = user.LastName,
                                                           Email = user.Email, UserTypeId = user.UserTypeId, UserId = user.UserId });
                return RedirectToAction("Index","Home");
            }
            return View(user);
        }
    }
}