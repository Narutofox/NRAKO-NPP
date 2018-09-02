using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRAKO_IvanCicek.Controllers;
using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;

namespace NPP_UnitTests
{
    [TestClass]
    public class UserControllerUnitTest
    {
        private UserController _controller;
        private LoginUser _loginUser;
        [TestInitialize]
        public void Initialize()
        {
            _loginUser = Helper.GetLoginUser();
            _controller = new UserController(Helper.GetContext(), _loginUser);
        }

        [TestMethod]
        public void Index()
        {
            ActionResult result = _controller.Index();

            Assert.IsNotNull(result);
            Assert.IsTrue(result is ViewResult);
            Assert.IsTrue((result as ViewResult).ViewName == "");
            Assert.IsNotNull((result as ViewResult).Model);
            Assert.IsTrue((result as ViewResult).Model is UserProfile);
            Assert.IsNotNull(_controller.ViewBag.VisibilityOptions);

            result = _controller.Index(2);

            Assert.IsNotNull(result);
            Assert.IsTrue(result is ViewResult);
            Assert.IsTrue((result as ViewResult).ViewName == "");
            Assert.IsNotNull((result as ViewResult).Model);
            Assert.IsTrue((result as ViewResult).Model is UserProfile);
            Assert.IsNotNull(_controller.ViewBag.VisibilityOptions);

            result = _controller.Index(-5);

            Assert.IsNotNull(result);
            Assert.IsTrue(result is RedirectToRouteResult);
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("action"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("controller"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["action"].ToString() == "Index");
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["controller"].ToString() == "Home");
            Assert.IsNotNull(_controller.ViewBag.VisibilityOptions);
        }

        [TestMethod]
        public void UserSearch()
        {
            JsonResult result = _controller.UserSearch("Ivan");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is IEnumerable<UserProfile>);
        }

        [TestMethod]
        public void ConfirmFriendRequest()
        {
            Context context = Helper.GetContext(true);
            int idUserToFriendList = 3;
            UserFriend userFriend = new UserFriend {IdUser = _loginUser.UserId, IdUserToFriendList = idUserToFriendList, RequestAccepted = false};

            context.UserFriends.Add(userFriend);
            context.SaveChanges();

            JsonResult result = _controller.ConfirmFriendRequest(userFriend.UserFriendId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");

            result = _controller.ConfirmFriendRequest(-1);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");

            context.UserFriends.Remove(userFriend);
            context.SaveChanges();
        }
    }
}
