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
    [TestClass]
    public class PostControllerUnitTests
    {
        private PostController _controller;
        private LoginUser _loginUser;
        private Mock<IPostsRepo> _postsRepoMock;
        [TestInitialize]
        public void Initialize()
        {
            _loginUser = Helper.GetLoginUser();
            _controller = new PostController(Helper.GetContext(), _loginUser);
            _postsRepoMock = new Mock<IPostsRepo>();
            _postsRepoMock.Setup(m => m.GetVisibilityOptions()).Returns(new List<Visibility>());
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
            CollectionAssert.AreEqual(_controller.PostsRepo.GetUserPosts(_loginUser.UserId).ToList(), (result.Model as IEnumerable<UserPost>).ToList());
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
            CollectionAssert.AreEqual(_controller.PostsRepo.GetProfileUserPosts(userId,_loginUser.UserId).ToList(), (result.Model as IEnumerable<UserPost>).ToList());
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
            CollectionAssert.AreEqual(_controller.PostsRepo.GetNews(_loginUser.UserId).ToList(), (result.Model as IEnumerable<UserPost>).ToList());
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

            JsonResult result = controller.CreatePost(newPost,null);
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

            JsonResult result = controller.CreatePost(newPost, null);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
            Assert.IsTrue((result.Data as JsonResponseVM).PostId == newPost.PostId);
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

            JsonResult result = controller.CreatePost(editPost, null);
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

            JsonResult result = controller.CreatePost(editPost, null);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "ERROR");
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

            JsonResult result = controller.EditPost(editPost, null);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);
            Assert.IsTrue(result.Data is JsonResponseVM);
            Assert.IsTrue((result.Data as JsonResponseVM).Result == "OK");
            Assert.IsTrue((result.Data as JsonResponseVM).PostId == editPost.PostId);
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
            List<PostCommentOrLike> list = new List<PostCommentOrLike>();
            list.Add(new PostCommentOrLike());

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
