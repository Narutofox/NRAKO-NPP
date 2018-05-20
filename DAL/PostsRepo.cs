using NRAKO_IvanCicek.Helpers;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace NRAKO_IvanCicek.DAL
{
    public class PostsRepo : IPostsRepo
    {
        readonly Context _context;
        private PostsRepo(Context context = null)
        {
            if (context != null)
            {
                _context = context;
            }
            else
            {
                _context = new Context();
            }

        }

        public static PostsRepo GetInstance(Context context = null)
        {
            return new PostsRepo(context);
        }



        public bool CreatePost(UserPost post)
        {
            post.PostDateTime = DateTime.Now;
            post.RecordStatusId = (int)RecordStatus.Active;
            _context.UserPosts.Add(post);
            return SaveChanges();

        }

        private bool SaveChanges()
        {
            if (_context.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        // Ovo je kada gledamo vlastiti profil
        public IEnumerable<UserPost> GetUserPosts(int userId)
        {
            return GetPosts(userId);
        }

        // Ovo je kada netko želi gledati tuđi profil
        public IEnumerable<UserPost> GetProfileUserPosts(int userId, int loginUserId)
        {
            return GetPosts(userId, loginUserId);
        }

        private IEnumerable<UserPost> GetPosts(int userId, int? loginUserId = null)
        {
            var list = (from ip in _context.UserPosts
                        join sp in _context.UserPosts on ip.SharedFromPostId equals sp.PostId into sfp
                        from sp in sfp.DefaultIfEmpty()
                        join u in _context.Users on ip.IdUser equals u.UserId
                        join spi in _context.Users on sp.IdUser equals spi.UserId into spin
                        from spi in spin.DefaultIfEmpty()
                        where ip.RecordStatusId == (int)RecordStatus.Active && ip.IdUser == userId 
                        && u.RecordStatusId == (int)RecordStatus.Active && ip.Verified
                        select new { UserPost = ip, SharedPost = sp, PostUser = u, ShareUser = spi });

            if (loginUserId.HasValue)
            {
                Boolean ShowFriendsOnlyData = _context.UserFriends.Any(x => x.RequestAccepted && ((x.IdUser == userId && x.IdUserToFriendList == loginUserId) || (x.IdUser == loginUserId && x.IdUserToFriendList == userId)));
                if (ShowFriendsOnlyData)
                {
                    list = list.Where(x => x.UserPost.Visibility == (int)Visibility.Javno || x.UserPost.Visibility == (int)Visibility.Samo_Prijatelji);
                }
                else
                {
                    list = list.Where(x => x.UserPost.Visibility == (int)Visibility.Javno);
                }
            }

            List<int> postIds = list.Select(x => x.UserPost.PostId).ToList();
            IList<PostCommentOrLike> commentOrLikes = _context.CommentOrLike.Where(x => postIds.Contains(x.IdPost) && x.RecordStatusId == (int)RecordStatus.Active).ToList();
            List<int> userIds = commentOrLikes.Select(x => x.IdUser).ToList();
            IList<User> commentUsers = _context.Users.Where(x => userIds.Contains(x.UserId)).ToList();
            foreach (var item in list)
            {
                if (item.UserPost.SharedFromPostId.HasValue && item.SharedPost != null)
                {
#pragma warning disable S1643 // Strings should not be concatenated using '+' in a loop
                    item.UserPost.Text += item.SharedPost.Text;
#pragma warning restore S1643 // Strings should not be concatenated using '+' in a loop
                }

                if (commentOrLikes.Any(x => x.IdPost == item.UserPost.PostId))
                {
                    var commentsList = commentOrLikes.Where(x => x.IdPost == item.UserPost.PostId).OrderByDescending(x => x.DateAndTime).ToList();
                    foreach (var commentOrLike in commentsList)
                    {
                        commentOrLike.UserFullName = commentUsers.Where(x => x.UserId == commentOrLike.IdUser).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                    }
                    item.UserPost.CommentsAndLikes = commentsList;
                }
                else
                {
                    item.UserPost.CommentsAndLikes = new List<PostCommentOrLike>();
                }

                item.UserPost.PostUser = item.PostUser;
                if (item.ShareUser != null)
                {
                    item.UserPost.SharedUser = item.ShareUser;
                }
            }
            return list.Select(x => x.UserPost).OrderByDescending(x => x.PostDateTime);
        }

        public IEnumerable<UserPost> GetNews(int userId)
        {
            var friendList = _context.UserFriends.Where(x => x.RequestAccepted && (x.IdUser == userId || x.IdUserToFriendList == userId)).ToList();
            var folowingList = _context.UsersFollowings.Where(x => x.IdUser == userId).ToList();

            IList<int> friendsUserIdsForPosts = new List<int>();
            IEnumerable<int> followingUserIdsForPosts = folowingList.Select(x => x.IdUserToFollow);
            foreach (var item in friendList)
            {
                if (item.IdUser != userId)
                {
                    friendsUserIdsForPosts.Add(item.IdUser);
                }
                else
                {
                    friendsUserIdsForPosts.Add(item.IdUserToFriendList);
                }
            }

            friendsUserIdsForPosts.Add(userId);
            var list = (from ip in _context.UserPosts
                                         join u in _context.Users on ip.IdUser equals u.UserId
                                         where u.RecordStatusId == (int)RecordStatus.Active && ip.RecordStatusId == (int)RecordStatus.Active && ip.Verified &&
                                         (
                                            friendsUserIdsForPosts.Contains(ip.IdUser) && (ip.Visibility == (int)Visibility.Javno || ip.Visibility == (int)Visibility.Samo_Prijatelji) ||
                                            followingUserIdsForPosts.Contains(ip.IdUser) && ip.Visibility == (int)Visibility.Javno
                                         )
                                         select new { Post = ip, User = u }).ToList();

            List<int> postIds = list.Select(x => x.Post.PostId).ToList();
            IList<PostCommentOrLike> commentOrLikes = _context.CommentOrLike.Where(x => postIds.Contains(x.IdPost) && x.RecordStatusId == (int)RecordStatus.Active).ToList();
            List<int> userIds = commentOrLikes.Select(x => x.IdUser).ToList();
            IList<User> commentUsers = _context.Users.Where(x => userIds.Contains(x.UserId)).ToList();

            foreach (var item in list)
            {
                item.Post.PostUser = item.User;

                if (commentOrLikes.Any(x => x.IdPost == item.Post.PostId))
                {
                    var commentsList = commentOrLikes.Where(x => x.IdPost == item.Post.PostId).OrderByDescending(x => x.DateAndTime).ToList();
                    foreach (var commentOrLike in commentsList)
                    {
                        commentOrLike.UserFullName = commentUsers.Where(x => x.UserId == commentOrLike.IdUser).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                    }
                    item.Post.CommentsAndLikes = commentsList;
                }
                else
                {
                    item.Post.CommentsAndLikes = new List<PostCommentOrLike>();
                }
            }

            return list.Select(x=>x.Post).OrderByDescending(x => x.PostDateTime);
        }

        public bool CreateCommentOrLike(PostCommentOrLike commentOrLike)
        {
            if (commentOrLike == null) throw new ArgumentNullException(nameof(commentOrLike));
            commentOrLike.DateAndTime = DateTime.Now;
            commentOrLike.RecordStatusId = (int)RecordStatus.Active;
            _context.CommentOrLike.Add(commentOrLike);
            return SaveChanges();
        }

        public bool UpdateCommentOrLike(PostCommentOrLike commentOrLike)
        {
            PostCommentOrLike commentOrLikeDB = _context.CommentOrLike.Where(x => x.PostCommentOrLikeId == commentOrLike.PostCommentOrLikeId && x.IdUser == commentOrLike.IdUser && x.RecordStatusId == (int)RecordStatus.Active).FirstOrDefault();
            if (commentOrLikeDB != null)
            {
                commentOrLikeDB.Comment = commentOrLike.Comment;
                commentOrLikeDB.DoYouLike = commentOrLike.DoYouLike;
                commentOrLikeDB.IdComment = commentOrLike.IdComment;
                commentOrLikeDB.DateAndTime = DateTime.Now;
                commentOrLikeDB.RecordStatusId = (int)RecordStatus.Active;
                _context.Entry(commentOrLikeDB).State = System.Data.Entity.EntityState.Modified;
                return SaveChanges();
            }
            return false;
        }

        public PostCommentOrLike GetCommentOrLike(int postCommentOrLikeId)
        {
            return _context.CommentOrLike.Where(x => x.PostCommentOrLikeId == postCommentOrLikeId && x.RecordStatusId == (int)RecordStatus.Active).FirstOrDefault();
        }

        public dynamic GetVisibilityOptions()
        {
            List<Visibility> Values = new List<Visibility>();
            foreach (Visibility item in Enum.GetValues(typeof(Visibility)))
            {
                Values.Add(item);
            }

            return from Visibility e in Values
                   select new { Id = (int)e, Name = e.ToString().Replace("_", " ") };
        }

        public bool EditPost(UserPost editPost)
        {
            UserPost Original = _context.UserPosts.Where(x => x.PostId == editPost.PostId && x.IdUser == editPost.IdUser && (x.Verified || editPost.Verified)).FirstOrDefault();
            if (Original != null)
            {
                editPost.PostDateTime = Original.PostDateTime;
                editPost.SharedFromPostId = Original.SharedFromPostId;
                editPost.RecordStatusId = Original.RecordStatusId;
                editPost.LastUpdate = DateTime.Now;
                _context.Entry(Original).State = System.Data.Entity.EntityState.Detached;
                _context.Entry(editPost).State = System.Data.Entity.EntityState.Modified;
                return SaveChanges();
            }
            return false;
        }

        public bool DeletePost(int postId,LoginUser user)
        {
            UserPost post = _context.UserPosts.FirstOrDefault(x => x.PostId == postId && (x.IdUser == user.UserId || (user.UserTypeId == (int)UserType.Admin && !x.Verified)));
            if (post == null) return false;
            post.RecordStatusId = (int)RecordStatus.Deleted;
            post.LastUpdate = DateTime.Now;
            _context.Entry(post).State = System.Data.Entity.EntityState.Modified;
            return SaveChanges();
        }

        public UserPost GetPost(int postId)
        {

            var Post = (from ip in _context.UserPosts
                        join sp in _context.UserPosts on ip.SharedFromPostId equals sp.PostId into sfp
                        from sp in sfp.DefaultIfEmpty()
                        join u in _context.Users on ip.IdUser equals u.UserId
                        join spi in _context.Users on sp.IdUser equals spi.UserId into spin
                        from spi in spin.DefaultIfEmpty()
                        where ip.RecordStatusId == (int)RecordStatus.Active && ip.PostId == postId && u.RecordStatusId == (int)RecordStatus.Active
                        select new { UserPost = ip, SharedPost = sp, PostUser = u, ShareUser = spi }).FirstOrDefault();

            if (Post != null)
            {
                IList<PostCommentOrLike> CommentOrLikes = GetCommentsAndLikes(postId).ToList();

                if (Post.UserPost.SharedFromPostId.HasValue && Post.SharedPost != null)
                {
                    Post.UserPost.Text += Post.SharedPost.Text;
                }

                if (CommentOrLikes.Any(x => x.IdPost == Post.UserPost.PostId))
                {
                    Post.UserPost.CommentsAndLikes = CommentOrLikes.Where(x => x.IdPost == Post.UserPost.PostId).OrderByDescending(x => x.DateAndTime).ToList();
                }
                else
                {
                    Post.UserPost.CommentsAndLikes = new List<PostCommentOrLike>();
                }

                Post.UserPost.PostUser = Post.PostUser;
                if (Post.ShareUser != null)
                {
                    Post.UserPost.SharedUser = Post.ShareUser;
                }

                return Post.UserPost;
            }
            return null;
        }

        public IEnumerable<PostCommentOrLike> GetCommentsAndLikes(int postId)
        {
            IList<PostCommentOrLike> CommentOrLikes = _context.CommentOrLike.Where(x => x.IdPost == postId && x.RecordStatusId == (int)RecordStatus.Active).OrderByDescending(x => x.DateAndTime).ToList() ;
            IEnumerable<int> UserIDs = CommentOrLikes.Select(x => x.IdUser).Distinct();
            IList<User> Users = _context.Users.Where(x => UserIDs.Contains(x.UserId)).ToList();
            foreach (var CommentOrLike in CommentOrLikes)
            {
                CommentOrLike.UserFullName = Users.Where(x => x.UserId == CommentOrLike.IdUser).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
            }

            return CommentOrLikes;
        }

        public bool DeleteComment(int postCommentOrLikeId, int userId)
        {
            PostCommentOrLike CommentOrLike = _context.CommentOrLike.FirstOrDefault(x => x.PostCommentOrLikeId == postCommentOrLikeId && x.IdUser == userId && x.RecordStatusId == (int)RecordStatus.Active);
            return DeleteCommentOrLike(CommentOrLike);
        }

        public bool DeleteLike(int postId, int loginUserId)
        {
            PostCommentOrLike CommentOrLike = _context.CommentOrLike.Where(x => x.IdPost == postId && x.IdUser == loginUserId && x.DoYouLike && x.RecordStatusId == (int)RecordStatus.Active).FirstOrDefault();
            return DeleteCommentOrLike(CommentOrLike);
        }

        private bool DeleteCommentOrLike(PostCommentOrLike commentOrLike)
        {
            if (commentOrLike != null)
            {
                commentOrLike.RecordStatusId = (int)RecordStatus.Deleted;
                _context.Entry(commentOrLike).State = System.Data.Entity.EntityState.Modified;
                return SaveChanges();
            }
            return false;
        }

        public IEnumerable<UserPost> GetUnverifiedPosts()
        {
            var posts = _context.UserPosts.Where(x => !x.Verified && x.RecordStatusId == (int)RecordStatus.Active).ToList();
            string canvasFilePath = String.Empty;
            foreach (var post in posts.Where(x=>!String.IsNullOrEmpty(x.CanvasJavascriptFilePath)))
            {
                canvasFilePath = HttpContext.Current.Server.MapPath(post.CanvasJavascriptFilePath);
                if (File.Exists(canvasFilePath))
                {
                    post.Canvas = File.ReadAllText(canvasFilePath, System.Text.Encoding.UTF8);
                }
            }
            return posts;
        }
    }
}