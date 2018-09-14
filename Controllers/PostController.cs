using NRAKO_IvanCicek.Factories;
using NRAKO_IvanCicek.Filters;
using NRAKO_IvanCicek.Helpers;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NRAKO_IvanCicek.Controllers
{
    [LoginRequiredFilter]
    public class PostController : Controller
    {
        public readonly IPostsRepo PostsRepo;
        private readonly LoginUser _loginUser;
        private readonly IFileSystem _fileSystem;

        public PostController()
        {
            PostsRepo = DalFactory.GetPostsRepo();
            _loginUser = (LoginUser)MySession.Get("LoginUser");
            _fileSystem = new FileSystem();
        }

        public PostController(Context context, LoginUser loginUser)
        {
            PostsRepo = DalFactory.GetPostsRepo(context);
            _loginUser = loginUser;
            _fileSystem = new FileSystem();
        }

        public PostController(IPostsRepo postsRepo, LoginUser loginUser, IFileSystem fileSystem = null)
        {
            PostsRepo = postsRepo;
            _loginUser = loginUser;
            _fileSystem = fileSystem ?? new FileSystem();
        }

        public PartialViewResult GetUserPosts(int? userId= null)
        {
            ViewBag.VisibilityOptions = PostsRepo.GetVisibilityOptions();
            IEnumerable<UserPost> posts;

            if (userId.HasValue && userId.Value != _loginUser.UserId)
            {
                posts = PostsRepo.GetProfileUserPosts(userId.Value, _loginUser.UserId);
            }
            else
            {
                posts = PostsRepo.GetUserPosts(_loginUser.UserId);
            }

            ViewBag.Scripts = posts.Where(x => !String.IsNullOrEmpty(x.CanvasJavascriptFilePath)).Select(x => x.CanvasJavascriptFilePath);
            return PartialView("_Posts", posts);
        }

        public PartialViewResult GetNews()
        {
            ViewBag.VisibilityOptions = PostsRepo.GetVisibilityOptions();
            IEnumerable<UserPost> news = PostsRepo.GetNews(_loginUser.UserId);
            ViewBag.Scripts = news.Where(x => !String.IsNullOrEmpty(x.CanvasJavascriptFilePath)).Select(x => x.CanvasJavascriptFilePath);
            return PartialView("_Posts", news);
        }

        public PartialViewResult AddNewPost(int postId)
        {
            UserPost post = PostsRepo.GetPost(postId);
            ViewBag.VisibilityOptions = PostsRepo.GetVisibilityOptions();
            if (post.Verified)
            {
                return PartialView("_AddNewPost", post);
            }
            else
            {
                return PartialView("_AddNewPost", null);
            }
           
        }

        [HttpPost]
        public JsonResult CreatePost(UserPost newPost, HttpPostedFileBase javascriptFile = null)
        {
            newPost.IdUser = _loginUser.UserId;

            if (javascriptFile != null && Path.GetExtension(javascriptFile.FileName) == ".js")
            {
                newPost.Verified = false;
                newPost.CanvasJavascriptFilePath = "~/Content/UserFiles/" + _loginUser.UserId + "/" + Guid.NewGuid().ToString()+ ".js";
            }

            if (ModelState.IsValid && PostsRepo.CreatePost(newPost))
            {
                if (javascriptFile != null && !String.IsNullOrEmpty(newPost.CanvasJavascriptFilePath))
                {
                    string folderPath = Server.MapPath(HardcodedValues.UserFiles + _loginUser.UserId);
                    if (!_fileSystem.Directory.Exists(folderPath))
                    {
                        _fileSystem.Directory.CreateDirectory(folderPath);
                    }
                    javascriptFile.SaveAs(Server.MapPath(newPost.CanvasJavascriptFilePath));
                    return Json(new JsonResponseVM { Result = "OK", Msg = "Vaša objava će biti dostupna nakon potvrde administratora" });
                }
                
                return Json(new JsonResponseVM { Result = "OK", PostId = newPost.PostId });
            }

            if (ModelState.Values.Any(x => x.Errors.Count > 0))
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = ModelState.Values.First(x => x.Errors.Count > 0).Errors.First().ErrorMessage });

            }
            return Json(new JsonResponseVM { Result = "ERROR", Msg = "CreatePost failed" });
        }

        [HttpGet]
        public JsonResult GetPost(int postId)
        {
            return Json(PostsRepo.GetPost(postId),JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditPost(UserPost editPost, HttpPostedFileBase javascriptFile = null)
        {
            editPost.IdUser = _loginUser.UserId;

            if (javascriptFile != null &&  Path.GetExtension(javascriptFile.FileName) == ".js")
            {
                editPost.Verified = false;
                editPost.CanvasJavascriptFilePath = "~/Content/UserFiles/" + _loginUser.UserId + "/" +Guid.NewGuid().ToString() + ".js";
            }

            if (ModelState.IsValid && PostsRepo.EditPost(editPost))
            {
                if (javascriptFile != null &&  !String.IsNullOrEmpty(editPost.CanvasJavascriptFilePath))
                {
                    string folderPath = Server.MapPath(HardcodedValues.UserFiles + _loginUser.UserId);
                    if (!_fileSystem.Directory.Exists(folderPath))
                    {
                        _fileSystem.Directory.CreateDirectory(folderPath);
                    }
                    javascriptFile.SaveAs(Server.MapPath(editPost.CanvasJavascriptFilePath));
                    return Json(new JsonResponseVM { Result = "OK", PostId = editPost.PostId, Msg = "Vaša objava će biti dostupna nakon potvrde administratora" });
                }

                return Json(new JsonResponseVM { Result = "OK", PostId = editPost.PostId });
            }
            else if (ModelState.Values.Any(x => x.Errors.Count > 0))
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = ModelState.Values.First(x => x.Errors.Count > 0).Errors.First().ErrorMessage });
            }
            else
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Traženi podatak ne postoji ili nemate potrebna prava." });
            }
        }

        [HttpPost]
        public JsonResult DeletePost(int postId)
        {
            
            if (PostsRepo.DeletePost(postId, _loginUser))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }
            else
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Traženi podatak ne postoji ili nemate potrebna prava." });
            }
        }


        public  PartialViewResult Comments (List<PostCommentOrLike> commentsAndLikes)
        {
            return PartialView("_PostComments", commentsAndLikes);
        }

        public PartialViewResult PostComments(int postId)
        {
            IEnumerable<PostCommentOrLike> list = PostsRepo.GetCommentsAndLikes(postId);
            return PartialView("_PostComments", list);
        }

        [HttpPost]
        public JsonResult CommentPost(string comment, int idPost)
        {          
            if (!String.IsNullOrEmpty(comment))
            {
                PostCommentOrLike postComment = new PostCommentOrLike { IdPost = idPost, Comment = comment, IdUser = _loginUser.UserId };
                if (PostsRepo.CreateCommentOrLike(postComment))
                {
                    return Json(new JsonResponseVM { Result = "OK", PostId = postComment.IdPost });
                }
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Pogreska" });
            }
            else
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Tekst komentara je obavezan" });
            }
            
        }

        [HttpPost]
        public JsonResult EditCommentPost(string comment, int postCommentOrLikeId, int idPost)
        {

            if (!String.IsNullOrEmpty(comment))
            {
                PostCommentOrLike commentModel = new PostCommentOrLike { Comment = comment, PostCommentOrLikeId = postCommentOrLikeId, IdUser = _loginUser.UserId };
                if (PostsRepo.UpdateCommentOrLike(commentModel))
                {
                    return Json(new JsonResponseVM { Result = "OK", PostId = idPost });
                }
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Pogreska" });
            }
            else
            {
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Tekst komentara je obavezan" });
            }

        }

        [HttpPost]
        public JsonResult DeleteCommentOrLike(int postCommentOrLikeId)
        {
               if( PostsRepo.DeleteComment(postCommentOrLikeId, _loginUser.UserId))
                {
                    return Json(new JsonResponseVM { Result = "OK" });
                }
            return Json(new JsonResponseVM { Result = "ERROR", Msg = "Pogreska" });
        }

        [HttpPost]
        public JsonResult ChekIfAnyMoreComments(int postId)
        {
            if (PostsRepo.GetCommentsAndLikes(postId).Any())
            {
                return Json(true);
            }
            return Json(false);
        }


        [HttpPost]
        public JsonResult Like(PostCommentOrLike like)
        {
            like.IdUser = _loginUser.UserId;
            like.DoYouLike = true;
            if (PostsRepo.CreateCommentOrLike(like)){
                return Json(new JsonResponseVM { Result = "OK" });
            }
            return Json(new JsonResponseVM { Result = "ERROR", Msg = "Pogreska" });
        }

        [HttpPost]
        public JsonResult Unlike(int postId)
        {
            if(PostsRepo.DeleteLike(postId, _loginUser.UserId))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }
            return Json(new JsonResponseVM { Result = "ERROR", Msg = "Pogreska" });
        }

        [AdminOnlyFilter]
        [HttpPost]
        public JsonResult AcceptPost(int postId)
        {
            UserPost post = PostsRepo.GetPost(postId);
            if (post != null)
            {
                post.Verified = true;
                if (PostsRepo.EditPost(post))
                {
                    return Json(new JsonResponseVM { Result = "OK" });
                }
                return Json(new JsonResponseVM { Result = "ERROR", Msg = "Pogreška kod spremanja" });
            }
            return Json(new JsonResponseVM { Result = "ERROR", Msg = "Pogreska" });
        }

        [AdminOnlyFilter]
        [HttpPost]
        public JsonResult DenyPost(int postId)
        {
            if (PostsRepo.DeletePost(postId, _loginUser))
            {
                return Json(new JsonResponseVM { Result = "OK" });
            }
            return Json(new JsonResponseVM { Result = "ERROR", Msg = "Pogreska" });
        }
    }
}