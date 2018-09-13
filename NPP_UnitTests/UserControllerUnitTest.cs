using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NRAKO_IvanCicek.Controllers;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;

namespace NPP_UnitTests
{
    [TestClass]
    public class UserControllerUnitTest
    {
        private UserController _controller;
        private LoginUser _loginUser;
        private Mock<IUserDAL> _userRepoMock;
        private Mock<IPostsRepo> _postsRepoMock;

        [TestInitialize]
        public void Initialize()
        {
            _loginUser = Helper.GetLoginUser();
            _controller = new UserController(Helper.GetContext(), _loginUser);
            _userRepoMock = new Mock<IUserDAL>();
            _postsRepoMock = new Mock<IPostsRepo>();
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
            int userFriendId = 3;

            _userRepoMock.Setup(m => m.ConfirmFriendRequest(userFriendId, _loginUser.UserId)).Returns(true);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);

            JsonResult result = controller.ConfirmFriendRequest(userFriendId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");


        }

        [TestMethod]
        public void ConfirmFriendRequest_DatabaseSaveFailed()
        {
            int userFriendId = 3;
            _userRepoMock.Setup(m => m.ConfirmFriendRequest(userFriendId, _loginUser.UserId)).Returns(false);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);

            JsonResult result = controller.ConfirmFriendRequest(userFriendId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void DenyFriendRequest()
        {
            int userFriendId = 3;

            _userRepoMock.Setup(m => m.DenyFriendRequest(userFriendId, _loginUser.UserId)).Returns(true);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);

            JsonResult result = controller.DenyFriendRequest(userFriendId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");

        }

        [TestMethod]
        public void DenyFriendRequest_DatabaseSaveFailed()
        {
            int userFriendId = 3;
            _userRepoMock.Setup(m => m.DenyFriendRequest(userFriendId, _loginUser.UserId)).Returns(false);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);

            JsonResult result = controller.DenyFriendRequest(userFriendId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void SendFriendRequest_IsOnFriendList()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.IsOnFriendList(userId, _loginUser.UserId)).Returns(true);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);

            JsonResult result = controller.SendFriendRequest(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR","Ne možeš dodati prijatelja ako već jeste prijatelji");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void SendFriendRequest_IsOnBlockList()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.IsOnFriendList(userId, _loginUser.UserId)).Returns(false);
            _userRepoMock.Setup(m => m.IsOnBlockList(userId, _loginUser.UserId)).Returns(true);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);

            JsonResult result = controller.SendFriendRequest(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR", "Ne možeš dodati prijatelja ako je blokiran od vas ili ste vi blokirani od njega/nje");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void SendFriendRequest_DatabaseSaveFailed()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.IsOnFriendList(userId, _loginUser.UserId)).Returns(false);
            _userRepoMock.Setup(m => m.IsOnBlockList(userId, _loginUser.UserId)).Returns(false);
            _userRepoMock.Setup(m => m.SendFriendRequest(userId, _loginUser.UserId)).Returns(false);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);

            JsonResult result = controller.SendFriendRequest(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void SendFriendRequest()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.IsOnFriendList(userId, _loginUser.UserId)).Returns(false);
            _userRepoMock.Setup(m => m.IsOnBlockList(userId, _loginUser.UserId)).Returns(false);
            _userRepoMock.Setup(m => m.SendFriendRequest(userId, _loginUser.UserId)).Returns(true);

            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);

            JsonResult result = controller.SendFriendRequest(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
        }

        [TestMethod]
        public void RemoveFriend_DatabaseSaveFailed()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.IsOnFriendList(userId, _loginUser.UserId)).Returns(false);
            _userRepoMock.Setup(m => m.RemoveFriend(userId, _loginUser.UserId)).Returns(false);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);

            JsonResult result = controller.RemoveFriend(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void RemoveFriend_IsOnFriendList()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.IsOnFriendList(userId, _loginUser.UserId)).Returns(false);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);

            JsonResult result = controller.RemoveFriend(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR", "Ne možeš ukloniti prijktelja ako već niste prijatelji");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void RemoveFriend()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.IsOnFriendList(userId, _loginUser.UserId)).Returns(true);
            _userRepoMock.Setup(m => m.RemoveFriend(userId, _loginUser.UserId)).Returns(true);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);

            JsonResult result = controller.RemoveFriend(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
        }

        [TestMethod]
        public void BlockUser_IsOnBlockList()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.IsOnBlockList(userId, _loginUser.UserId)).Returns(true);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);

            JsonResult result = controller.BlockUser(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR", "Ne možeš blokirati ako već je blokirano");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void BlockUser_DatabaseSaveFailed()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.IsOnBlockList(userId, _loginUser.UserId)).Returns(false);
            _userRepoMock.Setup(m => m.BlockUser(userId, _loginUser.UserId)).Returns(false);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);

            JsonResult result = controller.BlockUser(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void BlockUser()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.IsOnBlockList(userId, _loginUser.UserId)).Returns(false);
            _userRepoMock.Setup(m => m.BlockUser(userId, _loginUser.UserId)).Returns(true);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);

            JsonResult result = controller.BlockUser(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
        }


        [TestMethod]
        public void UnblockUser_IsOnBlockList()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.IsOnBlockList(userId, _loginUser.UserId)).Returns(false);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);

            JsonResult result = controller.UnblockUser(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR", "Ne možeš odblokirati ako već nije blokirano");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void UnblockUser_DatabaseSaveFailed()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.IsOnBlockList(userId, _loginUser.UserId)).Returns(true);
            _userRepoMock.Setup(m => m.UnblockUser(userId, _loginUser.UserId)).Returns(false);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);

            JsonResult result = controller.UnblockUser(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void UnblockUser()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.IsOnBlockList(userId, _loginUser.UserId)).Returns(true);
            _userRepoMock.Setup(m => m.UnblockUser(userId, _loginUser.UserId)).Returns(true);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);

            JsonResult result = controller.UnblockUser(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
        }

        [TestMethod]
        public void FollowUser_FolowNotAllowed()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.CanFollow(userId, _loginUser.UserId)).Returns(false);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);


            JsonResult result = controller.FollowUser(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void FollowUser_DatabaseSaveFailed()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.CanFollow(userId, _loginUser.UserId)).Returns(true);
            _userRepoMock.Setup(m => m.FollowUser(userId, _loginUser.UserId)).Returns(false);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);


            JsonResult result = controller.FollowUser(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }



        [TestMethod]
        public void FollowUser()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.CanFollow(userId, _loginUser.UserId)).Returns(true);
            _userRepoMock.Setup(m => m.FollowUser(userId, _loginUser.UserId)).Returns(true);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);


            JsonResult result = controller.FollowUser(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");       			      
        }



        [TestMethod]
        public void StopFollowingUser_IsFollowing()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.IsFollowing(userId, _loginUser.UserId)).Returns(false);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);


            JsonResult result = controller.StopFollowingUser(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR", "Ne možeš prestati pratiti korisnika kojeg ne pratiš");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void StopFollowingUser_DatabaseSaveFailed()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.IsFollowing(userId, _loginUser.UserId)).Returns(true);
            _userRepoMock.Setup(m => m.StopFollowingUser(userId, _loginUser.UserId)).Returns(false);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);


            JsonResult result = controller.StopFollowingUser(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }



        [TestMethod]
        public void StopFollowingUser()
        {
            int userId = 3;
            _userRepoMock.Setup(m => m.IsFollowing(userId, _loginUser.UserId)).Returns(true);
            _userRepoMock.Setup(m => m.StopFollowingUser(userId, _loginUser.UserId)).Returns(true);
            UserController controller = new UserController(_postsRepoMock.Object, _userRepoMock.Object, _loginUser);


            JsonResult result = controller.StopFollowingUser(userId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
        }
		
		[TestMethod]
        public void Friends()
        {
			ViewResult result = _controller.Friends();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "");
            Assert.IsNotNull(result.Model);
            Assert.IsTrue(result.Model is IEnumerable<UserFriend>);	
		}
		
		[TestMethod]
        public void AdminPanel()
        {
			ViewResult result = _controller.AdminPanel();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "");
            Assert.IsNotNull(result.Model);
            Assert.IsTrue(result.Model is IEnumerable<UserPost> );	
		}
    }
}
