using NRAKO_IvanCicek.Factories;
using NRAKO_IvanCicek.Filters;
using NRAKO_IvanCicek.Helpers;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NRAKO_IvanCicek.Controllers
{
    [LoginRequiredFilter]
    public class HomeController : Controller
    {
        IUserDAL userDAL;
        IPostsRepo _postsRepo;
        LoginUser LoginUser;
        public HomeController()
        {
            userDAL = DALFactory.GetUserDAL();
            _postsRepo = DALFactory.GetPostDAL();
            LoginUser = (LoginUser)MySession.Get("LoginUser");
        }
        public ActionResult Index()
        {
            ViewBag.VisibilityOptions = _postsRepo.GetVisibilityOptions();
            return View();
        }
    }
}