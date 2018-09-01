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
        
        private bool AllowSaveChanges = true;

        public Context() : base("CS")
        {

        }
        /// <summary>
        /// Konstruktor za unit testove
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="allowSaveChanges"></param>
        public Context(string connectionString, bool allowSaveChanges = false) : base(connectionString)
        {
            AllowSaveChanges = allowSaveChanges;
        }

        public override int SaveChanges()
        {
            if (AllowSaveChanges)
            {
                return base.SaveChanges();
            }
            return 1;
        }
    }
}