﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NRAKO_IvanCicek.Controllers;
using NRAKO_IvanCicek.Helpers;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;

namespace NPP_UnitTests
{
    [TestClass]
    public class PostControllerUnitTests
    {
        private LoginUser _loginUser;
        private Mock<IPostsRepo> _postsRepoMock;
        [TestInitialize]
        public void Initialize()
        {
            _loginUser = Helper.GetLoginUser();
            _postsRepoMock = new Mock<IPostsRepo>();
            _postsRepoMock.Setup(m => m.GetVisibilityOptions()).Returns(new List<EnumVM>().AsEnumerable());
        }

        [TestMethod]
        public void GetLoginUserPosts()
        {
            IEnumerable<UserPost> userPosts = new List<UserPost>();
            _postsRepoMock.Setup(m => m.GetUserPosts(_loginUser.UserId)).Returns(userPosts);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);
            PartialViewResult result = controller.GetUserPosts();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "_Posts");
            Assert.IsNotNull(result.Model);
            Assert.IsTrue(result.Model is IEnumerable<UserPost>);
            Assert.IsNotNull(controller.ViewBag.VisibilityOptions);
            Assert.IsTrue(controller.ViewBag.VisibilityOptions is IEnumerable<EnumVM>);
            Assert.IsTrue(controller.ViewBag.Scripts is IEnumerable<String>);
            CollectionAssert.AreEqual(userPosts.ToList(), (result.Model as IEnumerable<UserPost>).ToList());
        }

        [TestMethod]
        public void GetUserPosts()
        {
            int userId = 2;
            IEnumerable<UserPost> userPosts = new List<UserPost>();
            _postsRepoMock.Setup(m => m.GetUserPosts(userId)).Returns(userPosts);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);
            PartialViewResult result = controller.GetUserPosts(userId);
            

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "_Posts");
            Assert.IsNotNull(result.Model);
            Assert.IsTrue(result.Model is IEnumerable<UserPost>);
            Assert.IsNotNull(controller.ViewBag.VisibilityOptions);
            Assert.IsNotNull(controller.ViewBag.Scripts);
            Assert.IsTrue(controller.ViewBag.VisibilityOptions is IEnumerable<EnumVM>);
            Assert.IsTrue(controller.ViewBag.Scripts is IEnumerable<String>);
            CollectionAssert.AreEqual(userPosts.ToList(), (result.Model as IEnumerable<UserPost>).ToList());
        }


        [TestMethod]
        public void GetNews()
        {
            IEnumerable<UserPost> userPosts = new List<UserPost>();
            _postsRepoMock.Setup(m => m.GetNews(_loginUser.UserId)).Returns(userPosts);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);
            PartialViewResult result = controller.GetNews();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "_Posts");
            Assert.IsNotNull(result.Model);
            Assert.IsTrue(result.Model is IEnumerable<UserPost>);
            Assert.IsNotNull(controller.ViewBag.VisibilityOptions);
            Assert.IsNotNull(controller.ViewBag.Scripts);
            Assert.IsTrue(controller.ViewBag.VisibilityOptions is IEnumerable<EnumVM>);
            Assert.IsTrue(controller.ViewBag.Scripts is IEnumerable<String>);
            CollectionAssert.AreEqual(userPosts.ToList(), (result.Model as IEnumerable<UserPost>).ToList());
        }

        [TestMethod]
        public void AddNewPost_Verified()
        {
            int postId = -1;
            _postsRepoMock.Setup(m => m.GetPost(postId)).Returns(new UserPost { Verified = true, RecordStatusId = (int)RecordStatus.Active, IdUser = 2, Text = "Test",PostId = postId });
           
            PostController controller = new PostController(_postsRepoMock.Object,_loginUser);

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
            _postsRepoMock.Setup(m => m.GetPost(postId)).Returns(new UserPost { Verified = false, RecordStatusId = (int)RecordStatus.Active, IdUser = 2, Text = "Test", PostId = postId });
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            PartialViewResult result = controller.AddNewPost(postId);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "_AddNewPost");
            Assert.IsNull(result.Model);
            Assert.IsNotNull(controller.ViewBag.VisibilityOptions);
        }

        [TestMethod]
        public void CreatePost_ModelStateNotValid()
        {
            UserPost newPost = new UserPost
            {
                Verified = false,
                RecordStatusId = (int) RecordStatus.Active,
                IdUser = 2,
                Text = "Test",
                PostId = 1
            };
            _postsRepoMock.Setup(m => m.CreatePost(newPost)).Returns(true);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            controller.ModelState.AddModelError("UnitTest","TestError");

            JsonResult result = controller.CreatePost(newPost);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
            Assert.IsTrue((result.Data as JsonResponseVM).Msg == "TestError");
        }

        [TestMethod]
        public void CreatePost()
        {
            UserPost newPost = new UserPost
            {
                Verified = false,
                RecordStatusId = (int) RecordStatus.Active,
                IdUser = 2,
                Text = "Test",
                PostId = 1
            };
            _postsRepoMock.Setup(m => m.CreatePost(newPost)).Returns(true);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.CreatePost(newPost);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
            Assert.IsTrue((result.Data as JsonResponseVM).PostId == newPost.PostId);
        }

        [TestMethod]
        public void CreatePost_WithFile()
        {
            UserPost newPost = new UserPost
            {
                Verified = false,
                RecordStatusId = (int)RecordStatus.Active,
                IdUser = 2,
                Text = "Test",
                PostId = 1
            };

            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
            Mock<HttpPostedFileBase> file = new Mock<HttpPostedFileBase>();

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello"));

            file.Setup(x => x.InputStream).Returns(stream);
            file.Setup(x => x.ContentLength).Returns((int)stream.Length);
            file.Setup(x => x.FileName).Returns("Test.js");
            string basePath = @"c:\yourPath\App";

            contextMock.Setup(x => x.Server.MapPath(It.IsAny<String>())).Returns(basePath);
            file.Setup(x => x.SaveAs(It.IsAny<String>())).Verifiable();
           

            string folderPath = basePath + HardcodedValues.UserFiles + _loginUser.UserId;
            var fakeFileSystem = 
                new MockFileSystem(new Dictionary<string, MockFileData> { { folderPath, new MockDirectoryData() } });

            _postsRepoMock.Setup(m => m.CreatePost(newPost)).Returns(true);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser, fakeFileSystem);

            RequestContext rc = new RequestContext(contextMock.Object, new RouteData());
            controller.ControllerContext = new ControllerContext(rc, controller);

            JsonResult result = controller.CreatePost(newPost, file.Object);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void GetPost()
        {
            UserPost newPost = new UserPost
            {
                Verified = false,
                RecordStatusId = (int)RecordStatus.Active,
                IdUser = 2,
                Text = "Test",
                PostId = 1
            };
            _postsRepoMock.Setup(m => m.GetPost(newPost.PostId)).Returns(newPost);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.GetPost(newPost.PostId);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is UserPost);
        }

        [TestMethod]
        public void DeletePost()
        {
            int postId = 2;
            _postsRepoMock.Setup(m => m.DeletePost(postId,_loginUser)).Returns(true);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.DeletePost(postId);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
        }

        [TestMethod]
        public void DeletePost_Failed()
        {
            int postId = 2;
            _postsRepoMock.Setup(m => m.DeletePost(postId, _loginUser)).Returns(false);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.DeletePost(postId);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
        }

        [TestMethod]
        public void EditPost_ModelStateNotValid()
        {
            UserPost editPost = new UserPost
            {
                Verified = false,
                RecordStatusId = (int)RecordStatus.Active,
                IdUser = 2,
                Text = "Test",
                PostId = 1
            };
            _postsRepoMock.Setup(m => m.EditPost(editPost)).Returns(true);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            controller.ModelState.AddModelError("UnitTest", "TestError");

            JsonResult result = controller.EditPost(editPost);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
            Assert.IsTrue((result.Data as JsonResponseVM).Msg == "TestError");
        }


        [TestMethod]
        public void EditPost_DatabaseSaveFailed()
        {
            UserPost editPost = new UserPost
            {
                Verified = false,
                RecordStatusId = (int)RecordStatus.Active,
                IdUser = 2,
                Text = "Test",
                PostId = 1
            };
            _postsRepoMock.Setup(m => m.EditPost(editPost)).Returns(false);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.EditPost(editPost);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void EditPost()
        {
            UserPost editPost = new UserPost
            {
                Verified = false,
                RecordStatusId = (int)RecordStatus.Active,
                IdUser = 2,
                Text = "Test",
                PostId = 1
            };
            _postsRepoMock.Setup(m => m.EditPost(editPost)).Returns(true);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.EditPost(editPost);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
            Assert.IsTrue((result.Data as JsonResponseVM).PostId == editPost.PostId);
        }

        [TestMethod]
        public void EditPost_WithFile()
        {
            UserPost editPost = new UserPost
            {
                Verified = false,
                RecordStatusId = (int) RecordStatus.Active,
                IdUser = 2,
                Text = "Test",
                PostId = 1
            };

            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
            Mock<HttpPostedFileBase> file = new Mock<HttpPostedFileBase>();

            // The required properties from my Controller side
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello"));

            file.Setup(x => x.InputStream).Returns(stream);
            file.Setup(x => x.ContentLength).Returns((int) stream.Length);
            file.Setup(x => x.FileName).Returns("Test.js");
            string basePath = @"c:\yourPath\App";

            contextMock.Setup(x => x.Server.MapPath(It.IsAny<String>())).Returns(basePath);
            file.Setup(x => x.SaveAs(It.IsAny<String>())).Verifiable();


            string folderPath = basePath + HardcodedValues.UserFiles + _loginUser.UserId;
            var fakeFileSystem =
                new MockFileSystem(new Dictionary<string, MockFileData> {{folderPath, new MockDirectoryData()}});

            _postsRepoMock.Setup(m => m.EditPost(editPost)).Returns(true);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser, fakeFileSystem);

            RequestContext rc = new RequestContext(contextMock.Object, new RouteData());
            controller.ControllerContext = new ControllerContext(rc, controller);

            JsonResult result = controller.EditPost(editPost, file.Object);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void Comments()
        {

            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);
            List<PostCommentOrLike> list = new List<PostCommentOrLike>();
            PartialViewResult result = controller.Comments(list);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "_PostComments");
            Assert.IsNotNull(result.Model);
            Assert.IsTrue(result.Model is List<PostCommentOrLike>);    
            CollectionAssert.AreEqual(list, (result.Model as List<PostCommentOrLike>));

        }

        [TestMethod]
        public void PostComments()
        {
            int postId = 2;
            List<PostCommentOrLike> list = new List<PostCommentOrLike>();
            _postsRepoMock.Setup(m => m.GetCommentsAndLikes(postId)).Returns(list.AsEnumerable());
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);
            PartialViewResult result = controller.PostComments(postId);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "_PostComments");
            Assert.IsNotNull(result.Model);
            Assert.IsTrue(result.Model is IEnumerable<PostCommentOrLike>);
            CollectionAssert.AreEqual(list, (result.Model as List<PostCommentOrLike>));

        }


        [TestMethod]
        public void CommentPost_CommentTextRequired()
        {
            int postId = 2;
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);
            JsonResult result = controller.CommentPost("",postId);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
        }

        [TestMethod]
        public void CommentPost_DatabaseSaveFailed()
        {
            int postId = 2;
            string comment = "Hello";

            PostCommentOrLike postComment = new PostCommentOrLike { IdPost = postId, Comment = comment, IdUser=_loginUser.UserId };
            _postsRepoMock.Setup(m => m.CreateCommentOrLike(postComment)).Returns(false);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);
            JsonResult result = controller.CommentPost(comment, postId);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
        }

        [TestMethod]
        public void CommentPost()
        {
            int postId = 2;
            string comment = "Hello";

            _postsRepoMock.Setup(m => m.CreateCommentOrLike(It.IsAny<PostCommentOrLike>())).Returns(true);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.CommentPost(comment, postId);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
            Assert.IsTrue((result.Data as JsonResponseVM).PostId == postId);
        }


        [TestMethod]
        public void EditCommentPost_CommentTextRequired()
        {
            int postId = -2;
            int postCommentOrLikeId = -1;
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);
            JsonResult result = controller.EditCommentPost("", postCommentOrLikeId, postId);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
        }

        [TestMethod]
        public void EditCommentPost_DatabaseSaveFailed()
        {
            int postId = 2;
            int postCommentOrLikeId = 1;
            string comment = "Hello";

            PostCommentOrLike postComment = new PostCommentOrLike { IdPost = postId, Comment = comment, IdUser = _loginUser.UserId };
            _postsRepoMock.Setup(m => m.UpdateCommentOrLike(postComment)).Returns(false);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);
            JsonResult result = controller.EditCommentPost(comment, postCommentOrLikeId, postId);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
        }

        [TestMethod]
        public void EditCommentPost()
        {
            int postId = 2;
            string comment = "Hello";
            int postCommentOrLikeId = 1;

            _postsRepoMock.Setup(m => m.UpdateCommentOrLike(It.IsAny<PostCommentOrLike>())).Returns(true);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.EditCommentPost(comment, postCommentOrLikeId, postId);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
            Assert.IsTrue((result.Data as JsonResponseVM).PostId == postId);
        }

        [TestMethod]
        public void DeleteCommentOrLike_Failed()
        {
            int postCommentOrLikeId = 1;

            _postsRepoMock.Setup(m => m.DeleteComment(postCommentOrLikeId,_loginUser.UserId)).Returns(false);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.DeleteCommentOrLike(postCommentOrLikeId);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
        }

        [TestMethod]
        public void DeleteCommentOrLike()
        {
            int postCommentOrLikeId = 1;

            _postsRepoMock.Setup(m => m.DeleteComment(postCommentOrLikeId, _loginUser.UserId)).Returns(true);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.DeleteCommentOrLike(postCommentOrLikeId);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
        }


        [TestMethod]
        public void CheckIfAnyMoreComments()
        {
            int postId = 2;
            List<PostCommentOrLike> list = new List<PostCommentOrLike>
            {
                new PostCommentOrLike()
            };

            _postsRepoMock.Setup(m => m.GetCommentsAndLikes(postId)).Returns(list);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.ChekIfAnyMoreComments(postId);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is bool);
            Assert.IsTrue((bool)result.Data);
        }

        [TestMethod]
        public void CheckIfAnyMoreComments_Failed()
        {
            int postId = 2;

            _postsRepoMock.Setup(m => m.GetCommentsAndLikes(postId)).Returns(new List<PostCommentOrLike>());
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.ChekIfAnyMoreComments(postId);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is bool);
            Assert.IsFalse((bool)result.Data);
        }


        [TestMethod]
        public void Like()
        {
            _postsRepoMock.Setup(m => m.CreateCommentOrLike(It.IsAny<PostCommentOrLike>())).Returns(true);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.Like(new PostCommentOrLike());
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
        }

        [TestMethod]
        public void Like_DatabaseSaveFailed()
        {
            _postsRepoMock.Setup(m => m.CreateCommentOrLike(It.IsAny<PostCommentOrLike>())).Returns(false);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.Like(new PostCommentOrLike());
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
        }



        [TestMethod]
        public void Unlike()
        {
            int postId = 2;
            _postsRepoMock.Setup(m => m.DeleteLike(postId,_loginUser.UserId)).Returns(true);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.Unlike(postId);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
        }

        [TestMethod]
        public void Unlike_DatabaseSaveFailed()
        {
            int postId = 2;
            _postsRepoMock.Setup(m => m.DeleteLike(postId, _loginUser.UserId)).Returns(false);
            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.Unlike(postId);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void AcceptPost_PostDoesNotExist()
        {
            int postId = 2;
            _postsRepoMock.Setup(m => m.GetPost(postId)).Returns((UserPost)null);

            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.AcceptPost(postId);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void AcceptPost_DatabaseSaveFailed()
        {
            int postId = 2;
            _postsRepoMock.Setup(m => m.GetPost(postId)).Returns(new UserPost());
            _postsRepoMock.Setup(m => m.EditPost(It.IsAny<UserPost>())).Returns(false);

            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.AcceptPost(postId);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void AcceptPost()
        {
            int postId = 2;
            _postsRepoMock.Setup(m => m.GetPost(postId)).Returns(new UserPost());
            _postsRepoMock.Setup(m => m.EditPost(It.IsAny<UserPost>())).Returns(true);

            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.AcceptPost(postId);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
        }


        [TestMethod]
        public void DenyPost_DatabaseSaveFailed()
        {
            int postId = 2;
            _postsRepoMock.Setup(m => m.DeletePost(postId, _loginUser)).Returns(false);

            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.DenyPost(postId);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
            Assert.IsTrue(!String.IsNullOrEmpty((result.Data as JsonResponseVM).Msg));
        }

        [TestMethod]
        public void DenyPost()
        {
            int postId = 2;
            _postsRepoMock.Setup(m => m.DeletePost(postId,_loginUser)).Returns(true);

            PostController controller = new PostController(_postsRepoMock.Object, _loginUser);

            JsonResult result = controller.DenyPost(postId);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
        }
    }
}
