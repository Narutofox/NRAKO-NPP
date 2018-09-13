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
    public class UserController : Controller
    {
        IPostsRepo _postsRepo;
        IUserDAL UsersDAL;
        LoginUser LoginUser;
        public UserController()
        {
            _postsRepo = DALFactory.GetPostDAL();
            UsersDAL = DALFactory.GetUserDAL();
            LoginUser = (LoginUser)MySession.Get("LoginUser");
        }

        public UserController(Context context, LoginUser loginUser)
        {
            _postsRepo = DALFactory.GetPostDAL(context);
            UsersDAL = DALFactory.GetUserDAL(context);
            LoginUser = loginUser;
        }

        public UserController(IPostsRepo postsRepo,IUserDAL userRepo, LoginUser loginUser)
        {
            _postsRepo = postsRepo;
            UsersDAL = userRepo;
            LoginUser = loginUser;
        }


        // GET: User
        public ActionResult Index(int? id = null)
        {            
            UserProfile profile = UsersDAL.GetProfileData(id != null ? id.Value : LoginUser.UserId);
            if (profile == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id.HasValue && id.Value != LoginUser.UserId)
            {
                profile = UsersDAL.SetAdditionalSettingsForProfile(profile, LoginUser.UserId);
            }

            ViewBag.VisibilityOptions = _postsRepo.GetVisibilityOptions();
            return View(profile);       
        }

        [HttpPost]
        public JsonResult UserSearch(string fullName)
        {
            return Json(UsersDAL.Search(fullName));
        }

        [HttpPost]
        public JsonResult ConfirmFriendRequest(int userFriendId)
        {
            if (UsersDAL.ConfirmFriendRequest(userFriendId, LoginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }

            return ErrorJSONResponse();
        }

        [HttpPost]
        public JsonResult DenyFriendRequest(int userFriendId)
        {
            if (UsersDAL.DenyFriendRequest(userFriendId, LoginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }

            return ErrorJSONResponse();
        }


        [HttpPost]
        public JsonResult SendFriendRequest(int userId)
        {
            if (UsersDAL.IsOnFriendList(userId, LoginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Zahtjev za prijateljstvom već postoji" });
            }

            if(UsersDAL.IsOnBlockList(userId, LoginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Zahtjev se ne može izvršiti" });
            }

            if (UsersDAL.SendFriendRequest(userId, LoginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }

            return ErrorJSONResponse();
        }

        [HttpPost]
        public JsonResult RemoveFriend(int userId)
        {
            if (UsersDAL.IsOnFriendList(userId, LoginUser.UserId) == false)
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Zahtjev za prijateljstvom ne postoji" });
            }

            if (UsersDAL.RemoveFriend(userId, LoginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }

            return ErrorJSONResponse();

        }

        [HttpPost]
        public JsonResult BlockUser(int userId)
        {
            if (UsersDAL.IsOnBlockList(userId, LoginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Zahtjev se ne može izvršiti" });
            }

            if (UsersDAL.BlockUser(userId, LoginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }

            return ErrorJSONResponse();
        }

        [HttpPost]
        public JsonResult UnblockUser(int userId)
        {
            if (UsersDAL.IsOnBlockList(userId, LoginUser.UserId)== false)
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Zahtjev se ne može izvršiti" });
            }

            if (UsersDAL.UnblockUser(userId, LoginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }

            return ErrorJSONResponse();
        }

        [HttpPost]
        public JsonResult FollowUser(int userId)
        {
            if (UsersDAL.CanFollow(userId, LoginUser.UserId) == false)
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Zahtjev se ne može izvršiti" });
            }

            if (UsersDAL.FollowUser(userId, LoginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }

            return ErrorJSONResponse();
        }


        [HttpPost]
        public JsonResult StopFollowingUser(int userId)
        {
            if (UsersDAL.IsFollowing(userId, LoginUser.UserId) == false)
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Zahtjev se ne može izvršiti" });
            }

            if (UsersDAL.StopFollowingUser(userId, LoginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }

            return ErrorJSONResponse();
        }



        public ViewResult Friends()
        {            
            return View(UsersDAL.GetFriends(LoginUser.UserId));
        }

        [AdminOnlyFilter]
        public ViewResult AdminPanel()
        {
            return View(_postsRepo.GetUnverifiedPosts());
        }

        

        private JsonResult ErrorJSONResponse()
        {
            return Json(new JsonResponseVM { Result = "ERROR", Msg = "Dogodila se pogreška" });
        }
    }
}