﻿using NRAKO_IvanCicek.Factories;
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
      
        // GET: User
        public ActionResult Index(int? id = null)
        {
            ViewBag.VisibilityOptions = _postsRepo.GetVisibilityOptions();

            UserProfile Profile = UsersDAL.GetProfileData(id != null ? id.Value : LoginUser.UserId);
            if (id.HasValue && id.Value != LoginUser.UserId)
            {
                Profile = UsersDAL.SetAdditionalSettingsForProfile(Profile, LoginUser.UserId);
            }
            return View(Profile);       
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
                return Json(new { Result = "OK" });
            }

            return ErrorJSONResponse();
        }

        [HttpPost]
        public JsonResult DenyFriendRequest(int userFriendId)
        {
            if (UsersDAL.DenyFriendRequest(userFriendId, LoginUser.UserId))
            {
                return Json(new { Result = "OK" });
            }

            return ErrorJSONResponse();
        }


        [HttpPost]
        public JsonResult SendFriendRequest(int userId)
        {
            if (UsersDAL.IsOnFriendList(userId, LoginUser.UserId))
            {
                return Json(new { Result = "ERROR", Msg = "Zahtjev za prijateljstvom već postoji" });
            }

            if(UsersDAL.IsOnBlockList(userId, LoginUser.UserId))
            {
                return Json(new { Result = "ERROR", Msg = "Zahtjev se ne može izvršiti" });
            }

            if (UsersDAL.SendFriendRequest(userId, LoginUser.UserId))
            {
                return Json(new { Result = "OK" });
            }

            return ErrorJSONResponse();
        }

        [HttpPost]
        public JsonResult RemoveFriend(int userId)
        {
            if (UsersDAL.IsOnFriendList(userId, LoginUser.UserId) == false)
            {
                return Json(new { Result = "ERROR", Msg = "Zahtjev za prijateljstvom ne postoji" });
            }

            if (UsersDAL.RemoveFriend(userId, LoginUser.UserId))
            {
                return Json(new { Result = "OK" });
            }

            return ErrorJSONResponse();

        }

        [HttpPost]
        public JsonResult BlockUser(int userId)
        {
            if (UsersDAL.IsOnBlockList(userId, LoginUser.UserId))
            {
                return Json(new { Result = "ERROR", Msg = "Zahtjev se ne može izvršiti" });
            }

            if (UsersDAL.BlockUser(userId, LoginUser.UserId))
            {
                return Json(new { Result = "OK" });
            }

            return ErrorJSONResponse();
        }

        [HttpPost]
        public JsonResult UnblockUser(int userId)
        {
            if (UsersDAL.IsOnBlockList(userId, LoginUser.UserId)== false)
            {
                return Json(new { Result = "ERROR", Msg = "Zahtjev se ne može izvršiti" });
            }

            if (UsersDAL.UnblockUser(userId, LoginUser.UserId))
            {
                return Json(new { Result = "OK" });
            }

            return ErrorJSONResponse();
        }

        [HttpPost]
        public JsonResult FollowUser(int userId)
        {
            if (UsersDAL.CanFollow(userId, LoginUser.UserId) == false)
            {
                return Json(new { Result = "ERROR", Msg = "Zahtjev se ne može izvršiti" });
            }

            if (UsersDAL.FollowUser(userId, LoginUser.UserId))
            {
                return Json(new { Result = "OK" });
            }

            return ErrorJSONResponse();
        }


        [HttpPost]
        public JsonResult StopFollowingUser(int userId)
        {
            if (UsersDAL.IsFolowing(userId, LoginUser.UserId) == false)
            {
                return Json(new { Result = "ERROR", Msg = "Zahtjev se ne može izvršiti" });
            }

            if (UsersDAL.StopFollowingUser(userId, LoginUser.UserId))
            {
                return Json(new { Result = "OK" });
            }

            return ErrorJSONResponse();
        }



        public ActionResult Friends()
        {            
            return View(UsersDAL.GetFriends(LoginUser.UserId));
        }

        [AdminOnlyFilter]
        public ActionResult AdminPanel()
        {
            return View(_postsRepo.GetUnverifiedPosts());
        }

        

        private JsonResult ErrorJSONResponse()
        {
            return Json(new { Result = "ERROR", Msg = "Dogodila se pogreška" });
        }
    }
}