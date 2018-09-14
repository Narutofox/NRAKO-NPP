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
using NRAKO_IvanCicek.Filters;

namespace NRAKO_IvanCicek.Controllers
{
    [SignUpControllerFilter]
    public class SignUpController : Controller
    {
        readonly IUserDAL userDAL;
        public SignUpController()
        {
            userDAL = DALFactory.GetUserDAL();
        }

        public SignUpController(Context context)
        {
            userDAL = DALFactory.GetUserDAL(context);
        }

        // GET: SignUp
        public ViewResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(SignUp signUpUser)
        {
            if (ModelState.IsValid)
            {
                User user = new User()
                {
                    Email = signUpUser.Email,
                    FirstName = signUpUser.FirstName,
                    LastName = signUpUser.LastName,
                    Password = signUpUser.Password
                };
                
                user.Salt = Hashing.GetSalt();
                user.Password = Hashing.Hash(user.Password, user.Salt);
                user.RecordStatusId = (int)RecordStatus.Active;
                user.UserTypeId = (int)UserType.Normal;

                if (userDAL.Create(user))
                {
                    MySession.Set("LoginUser", new LoginUser
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        UserTypeId = user.UserTypeId,
                        UserId = user.UserId
                    });
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["notification"] = new Notification()
                    {
                        Type = NotificationType.Error,
                        Text = "Dogodila se pogreška tokom registracije"
                    };
                }
            }
            return View("Index", signUpUser);
        }
    }
}