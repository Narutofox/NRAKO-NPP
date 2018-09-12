using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NRAKO_IvanCicek.Controllers;
using NRAKO_IvanCicek.Helpers;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;

namespace NPP_UnitTests
{
    //TODO
    [TestClass]
    public class PostControllerUnitTests
    {
        private PostController _controller;
        private LoginUser _loginUser;
        private Mock<IPostsRepo> myInterfaceMock;
        [TestInitialize]
        public void Initialize()
        {
            _loginUser = Helper.GetLoginUser();
            _controller = new PostController(Helper.GetContext(), _loginUser);
            myInterfaceMock = new Mock<IPostsRepo>();
            myInterfaceMock.Setup(m => m.GetVisibilityOptions()).Returns(new List<Visibility>());
        }

        [TestMethod]
        public void GetLoginUserPosts()
        {
            PartialViewResult result = _controller.GetUserPosts();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "_Posts");
            Assert.IsNotNull(result.Model);
            Assert.IsTrue(result.Model is IEnumerable<UserPost>);
            Assert.IsNotNull(_controller.ViewBag.VisibilityOptions);
            Assert.IsNotNull(_controller.ViewBag.Scripts);
            CollectionAssert.AreEqual(_controller._postsRepo.GetUserPosts(_loginUser.UserId).ToList(), (result.Model as IEnumerable<UserPost>).ToList());
        }

        [TestMethod]
        public void GetUserPosts()
        {
            int userId = 2;
            PartialViewResult result = _controller.GetUserPosts(userId);
            

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "_Posts");
            Assert.IsNotNull(result.Model);
            Assert.IsTrue(result.Model is IEnumerable<UserPost>);
            Assert.IsNotNull(_controller.ViewBag.VisibilityOptions);
            Assert.IsNotNull(_controller.ViewBag.Scripts);
            CollectionAssert.AreEqual(_controller._postsRepo.GetProfileUserPosts(userId,_loginUser.UserId).ToList(), (result.Model as IEnumerable<UserPost>).ToList());
        }


        [TestMethod]
        public void GetNews()
        {
            PartialViewResult result = _controller.GetNews();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "_Posts");
            Assert.IsNotNull(result.Model);
            Assert.IsTrue(result.Model is IEnumerable<UserPost>);
            Assert.IsNotNull(_controller.ViewBag.VisibilityOptions);
            Assert.IsNotNull(_controller.ViewBag.Scripts);
            CollectionAssert.AreEqual(_controller._postsRepo.GetNews(_loginUser.UserId).ToList(), (result.Model as IEnumerable<UserPost>).ToList());
        }

        [TestMethod]
        public void AddNewPost_Verified()
        {
            int postId = -1;
            myInterfaceMock.Setup(m => m.GetPost(postId)).Returns(new UserPost { Verified = true, RecordStatusId = (int)RecordStatus.Active, IdUser = 2, Text = "Test",PostId = postId });
           
            PostController controller = new PostController(myInterfaceMock.Object,_loginUser);

            PartialViewResult result = controller.AddNewPost(postId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "_AddNewPost");
            Assert.IsNotNull(result.Model);
            Assert.IsTrue(result.Model is UserPost);
            Assert.IsTrue((result.Model as UserPost).PostId == postId);
            Assert.IsNotNull(controller.ViewBag.VisibilityOptions);
        }

        [TestMethod]
        public void AddNewPost_NotVerified()
        {
            int postId = -1;
            myInterfaceMock.Setup(m => m.GetPost(postId)).Returns(new UserPost { Verified = false, RecordStatusId = (int)RecordStatus.Active, IdUser = 2, Text = "Test", PostId = postId });
            PostController controller = new PostController(myInterfaceMock.Object, _loginUser);

            PartialViewResult result = controller.AddNewPost(postId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "_AddNewPost");
            Assert.IsNull(result.Model);
            Assert.IsNotNull(controller.ViewBag.VisibilityOptions);
        }
    }
}
