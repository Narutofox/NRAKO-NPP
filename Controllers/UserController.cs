using NRAKO_IvanCicek.Factories;
using NRAKO_IvanCicek.Filters;
using NRAKO_IvanCicek.Helpers;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;
using System.Web.Mvc;

namespace NRAKO_IvanCicek.Controllers
{
    [LoginRequiredFilter]
    public class UserController : Controller
    {
        private readonly IPostsRepo _postsRepo;
        private readonly IUserDAL _usersDal;
        private readonly LoginUser _loginUser;
        public UserController()
        {
            _postsRepo = DalFactory.GetPostsRepo();
            _usersDal = DalFactory.GetUsersRepo();
            _loginUser = (LoginUser)MySession.Get("LoginUser");
        }

        public UserController(IPostsRepo postsRepo,IUserDAL userRepo, LoginUser loginUser)
        {
            _postsRepo = postsRepo;
            _usersDal = userRepo;
            _loginUser = loginUser;
        }


        // GET: User
        public ActionResult Index(int? id = null)
        {            
            UserProfile profile = _usersDal.GetProfileData(id ?? _loginUser.UserId);
            if (profile == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (id.HasValue && id.Value != _loginUser.UserId)
            {
                profile = _usersDal.SetAdditionalSettingsForProfile(profile, _loginUser.UserId);
            }

            ViewBag.VisibilityOptions = _postsRepo.GetVisibilityOptions();
            return View(profile);       
        }

        [HttpPost]
        public JsonResult UserSearch(string fullName)
        {
            return Json(_usersDal.Search(fullName));
        }

        [HttpPost]
        public JsonResult ConfirmFriendRequest(int userFriendId)
        {
            if (_usersDal.ConfirmFriendRequest(userFriendId, _loginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }

            return ErrorJsonResponse();
        }

        [HttpPost]
        public JsonResult DenyFriendRequest(int userFriendId)
        {
            if (_usersDal.DenyFriendRequest(userFriendId, _loginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }

            return ErrorJsonResponse();
        }


        [HttpPost]
        public JsonResult SendFriendRequest(int userId)
        {
            if (_usersDal.IsOnFriendList(userId, _loginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Zahtjev za prijateljstvom već postoji" });
            }

            if(_usersDal.IsOnBlockList(userId, _loginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Zahtjev se ne može izvršiti" });
            }

            if (_usersDal.SendFriendRequest(userId, _loginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }

            return ErrorJsonResponse();
        }

        [HttpPost]
        public JsonResult RemoveFriend(int userId)
        {
            if (_usersDal.IsOnFriendList(userId, _loginUser.UserId) == false)
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Zahtjev za prijateljstvom ne postoji" });
            }

            if (_usersDal.RemoveFriend(userId, _loginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }

            return ErrorJsonResponse();

        }

        [HttpPost]
        public JsonResult BlockUser(int userId)
        {
            if (_usersDal.IsOnBlockList(userId, _loginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Zahtjev se ne može izvršiti" });
            }

            if (_usersDal.BlockUser(userId, _loginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }

            return ErrorJsonResponse();
        }

        [HttpPost]
        public JsonResult UnblockUser(int userId)
        {
            if (_usersDal.IsOnBlockList(userId, _loginUser.UserId)== false)
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Zahtjev se ne može izvršiti" });
            }

            if (_usersDal.UnblockUser(userId, _loginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }

            return ErrorJsonResponse();
        }

        [HttpPost]
        public JsonResult FollowUser(int userId)
        {
            if (_usersDal.CanFollow(userId, _loginUser.UserId) == false)
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Zahtjev se ne može izvršiti" });
            }

            if (_usersDal.FollowUser(userId, _loginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }

            return ErrorJsonResponse();
        }


        [HttpPost]
        public JsonResult StopFollowingUser(int userId)
        {
            if (_usersDal.IsFollowing(userId, _loginUser.UserId) == false)
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Zahtjev se ne može izvršiti" });
            }

            if (_usersDal.StopFollowingUser(userId, _loginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }

            return ErrorJsonResponse();
        }



        public ViewResult Friends()
        {            
            return View(_usersDal.GetFriends(_loginUser.UserId));
        }

        [AdminOnlyFilter]
        public ViewResult AdminPanel()
        {
            return View(_postsRepo.GetUnverifiedPosts());
        }

        

        private JsonResult ErrorJsonResponse()
        {
            return Json(new JsonResponseVM { Result = "ERROR", Msg = "Dogodila se pogreška" });
        }
    }
}