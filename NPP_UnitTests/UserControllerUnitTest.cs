using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
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
		
		[TestMethod]
        public void DenyFriendRequest()
        {
            Context context = Helper.GetContext(true);
            int idUserToFriendList = 3;
            UserFriend userFriend = new UserFriend {IdUser = _loginUser.UserId, IdUserToFriendList = idUserToFriendList, RequestAccepted = false};

            context.UserFriends.Add(userFriend);
            context.SaveChanges();

            JsonResult result = _controller.DenyFriendRequest(userFriend.UserFriendId);
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
		
		[TestMethod]
        public void SendFriendRequest()
        {
            Context context = Helper.GetContext(true);
            int idUserToFriendList = 5;
            UserFriend userFriend = new UserFriend {IdUser = _loginUser.UserId, IdUserToFriendList = idUserToFriendList, RequestAccepted = false};

            context.UserFriends.Add(userFriend);
            context.SaveChanges();

            JsonResult result = _controller.SendFriendRequest(idUserToFriendList);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR","Ne možeš poslati zahtjev ako već jeste prijatelji ");
			
						
			UserBlacklist userblock = new UserBlacklist {IdUser = _loginUser.UserId, IdUserToBlackList = idUserToFriendList};

            context.UserBlacklists.Add(userblock);
			context.UserFriends.Remove(userFriend);
            context.SaveChanges();

            result = _controller.SendFriendRequest(idUserToFriendList);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR","Ne možeš poslati zahtjev ako si na block listi");

			
			context.UserBlacklists.Remove(userblock);
            context.SaveChanges();
			
			result = _controller.SendFriendRequest(idUserToFriendList);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
        }
		
		[TestMethod]
        public void RemoveFriend()
        {
            Context context = Helper.GetContext(true);
            int idUserToFriendList = 5;
            
            JsonResult result = _controller.RemoveFriend(idUserToFriendList);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR","Ne možeš maknuti sa liste prijatelja ako već niste prijatelji");
			
			UserFriend userFriend = new UserFriend {IdUser = _loginUser.UserId, IdUserToFriendList = idUserToFriendList, RequestAccepted = true};

            context.UserFriends.Add(userFriend);
            context.SaveChanges();
			
			result = _controller.RemoveFriend(idUserToFriendList);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");

            context.UserFriends.Remove(userFriend);
            context.SaveChanges();
        }
		
		[TestMethod]
        public void BlockUser()
        {
            Context context = Helper.GetContext(true);
            int idUserToBlackList = 3;
									
			UserBlacklist userblock = new UserBlacklist {IdUser = _loginUser.UserId, IdUserToBlackList = idUserToBlackList};

            context.UserBlacklists.Add(userblock);
            context.SaveChanges();

            JsonResult result = _controller.BlockUser(idUserToBlackList);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR","Ne možeš blokirati ako već je blokirano");

			
			context.UserBlacklists.Remove(userblock);
            context.SaveChanges();
			
			result = _controller.BlockUser(idUserToBlackList);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
        }
		
		[TestMethod]
        public void UnblockUser()
        {
            Context context = Helper.GetContext(true);
            int idUserToBlackList = 3;
											          
            JsonResult result = _controller.UnblockUser(idUserToBlackList);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR","Ne možeš odblokirati ako već nije blokirano");

			UserBlacklist userblock = new UserBlacklist {IdUser = _loginUser.UserId, IdUserToBlackList = idUserToBlackList};
			context.UserBlacklists.Add(userblock);
            context.SaveChanges();
						
			result = _controller.UnblockUser(idUserToBlackList);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
			
			context.UserBlacklists.Remove(userblock);
            context.SaveChanges();
        }
		
		[TestMethod]
        public void FollowUser()
        {
            Context context = Helper.GetContext(true);
            int idUser = 5;
			bool allowFolowOriginalState = true;

            UserBlacklist userblock = new UserBlacklist {IdUser = _loginUser.UserId, IdUserToBlackList = idUser};
			context.UserBlacklists.Add(userblock);
            context.SaveChanges();
			
			JsonResult result = _controller.FollowUser(idUser);
			Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR","Ne možeš pratiti korisnika kojeg si blokritao ili koji je tebe blokirao");
			
			context.UserBlacklists.Remove(userblock);
            context.SaveChanges();
			
			UserFollow userFollow= new UserFollow {IdUser = _loginUser.UserId, IdUserToFollow = idUser};
			context.UsersFollowings.Add(userFollow);
            context.SaveChanges();
			
			result = _controller.FollowUser(idUser);
			Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR","Ne možeš pratiti korisnika kojeg već pratiš");
			
			context.UsersFollowings.Remove(userFollow);
            context.SaveChanges();
			
			UserSetting userSettings = context.UserSettings.First(x=>x.IdUser == idUser);
			allowFolowOriginalState = userSettings.AllowFollowing;
            userSettings.AllowFollowing = false;

            context.Entry(userSettings).State = EntityState.Modified;
            if (context.SaveChanges() > 0)
            {
                result = _controller.FollowUser(idUser);
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Data is JsonResponseVM);
                Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR", "Ne možeš pratiti korisnika koji ne dozvoljava pračenje");
            }

            userSettings = context.UserSettings.First(x => x.IdUser == idUser);
            userSettings.AllowFollowing = true;
			
			context.Entry(userSettings).State = EntityState.Modified;
            if (context.SaveChanges() > 0)
            {
                result = _controller.FollowUser(idUser);
                Assert.IsNotNull(result);
                Assert.IsTrue(result.Data is JsonResponseVM);
                Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
            }
			
			
			
			userSettings.AllowFollowing = allowFolowOriginalState;
			context.Entry(userSettings).State = EntityState.Modified;
			context.SaveChanges();
		}
		
		
		[TestMethod]
        public void StopFollowingUser()
        {
			int idUser = 5;
			bool allowFolowOriginalState = true;
            Context context = Helper.GetContext(true);

            JsonResult result = _controller.StopFollowingUser(idUser);
			Assert.IsNotNull(result);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR","Ne možeš prestati pratiti korisnika kojeg ne pratiš");

			
			result = _controller.FollowUser(idUser);
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
