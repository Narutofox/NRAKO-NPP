using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace NRAKO_IvanCicek.Models
{
    public class Context : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<UserPost> UserPosts { get; set; }
        public DbSet<UserFriend> UserFriends { get; set; }
        public DbSet<UserSetting> UserSettings { get; set; }
        public DbSet<PostCommentOrLike> CommentOrLike { get; set; }
        public DbSet<UserBlacklist> UserBlacklists { get; set; }
        public DbSet<UserFollow> UsersFollowings { get; set; }
        
        public Context() : base("CS")
        {

        }

        public Context(string connectionString) : base(connectionString)
        {
        }
    }
}