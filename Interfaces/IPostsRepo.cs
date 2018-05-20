using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NRAKO_IvanCicek.Interfaces
{
    public interface IPostsRepo
    {
        bool CreatePost(UserPost post);
        IEnumerable<UserPost> GetUserPosts(int userId);
        IEnumerable<UserPost> GetProfileUserPosts(int userId, int loginUserId);
        IEnumerable<UserPost> GetNews(int userId);
        bool CreateCommentOrLike(PostCommentOrLike commentOrLike);
        bool UpdateCommentOrLike(PostCommentOrLike commentOrLike);
        PostCommentOrLike GetCommentOrLike(int postCommentOrLikeId);
        dynamic GetVisibilityOptions();
        bool EditPost(UserPost editPost);
        UserPost GetPost(int postId);
        bool DeletePost(int postId, LoginUser user);
        IEnumerable<PostCommentOrLike> GetCommentsAndLikes(int postId);
        bool DeleteComment(int postCommentOrLikeId, int userId);
        bool DeleteLike(int postId, int loginUserId);
        IEnumerable<UserPost> GetUnverifiedPosts();
    }
}
