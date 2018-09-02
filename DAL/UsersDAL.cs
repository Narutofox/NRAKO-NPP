using NRAKO_IvanCicek.Helpers;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using NRAKO_IvanCicek.Models.VM;

namespace NRAKO_IvanCicek.DAL
{
    public class UsersDAL : IUserDAL
    {
        Context Context;
        private UsersDAL(Context context = null)
        {
            if (context != null)
            {
                Context = context;
            }
            else
            {
                Context = new Context();
            }
        }

        public static UsersDAL GetInstance(Context context = null)
        {
            return new UsersDAL(context);
        }

        private bool SaveChanges()
        {
            if (Context.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        public bool Create(User user)
        {
            user.RecordStatusId = (int)RecordStatus.Active;
            Context.Users.Add(user);
            Context.SaveChanges();
            Context.UserSettings.Add(new UserSetting { IdUser = user.UserId, AllowFollowing = false, ShowEmail = false });
            if (Context.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        public bool Update(User user)
        {
            User DbUser = Get(user.UserId);
            if (DbUser != null)
            {
                // Ovdje se lozinka i Salt NE mijenja
                user.Password = DbUser.Password;
                user.Salt = DbUser.Salt;
                // Ako je polje za putanju do slike profila prazno onda postavi onu vrijednost koja je u bazi
                if (String.IsNullOrEmpty(user.ProfileImagePath))
                {
                    user.ProfileImagePath = DbUser.ProfileImagePath;
                }

                if (user.RecordStatusId <= 0)
                {
                    user.RecordStatusId = (int)RecordStatus.Active;
                }

                Context.Entry(DbUser).State = EntityState.Detached;
                Context.Entry(user).State = EntityState.Modified;
                if (Context.SaveChanges() > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Delete(User user)
        {
            user.RecordStatusId = (int)RecordStatus.Deleted;
            return Update(user);
        }



        public UserSetting GetUserSettings(int userId)
        {
            return Context.UserSettings.Where(x => x.IdUser == userId).FirstOrDefault();
        }

        public User Get(int userId)
        {
            return Context.Users.Where(x => x.UserId == userId && x.RecordStatusId == (int)RecordStatus.Active).FirstOrDefault();
        }

        public UserProfile GetProfileData(int userId)
        {
            User user = Context.Users.Where(x => x.UserId == userId && x.RecordStatusId == (int)RecordStatus.Active).FirstOrDefault();

            if (user != null)
            {
                UserProfile Profile = new Models.UserProfile();
                Profile.UserId = user.UserId;
                Profile.FirstName = user.FirstName;
                Profile.LastName = user.LastName;
                Profile.ProfileImagePath = user.ProfileImagePath;

                UserSetting Settings = GetUserSettings(userId);

                Profile.AllowFollowing = Settings.AllowFollowing;
                Profile.ShowEmail = Settings.AllowFollowing;
                return Profile;
            }
            return null;
        }

        public bool UpdateUserSettings(UserSetting userSettings)
        {
            UserSetting DBUserSettings = GetUserSettings(userSettings.IdUser);

            if (DBUserSettings != null)
            {
                userSettings.UserSettingId = DBUserSettings.UserSettingId;

                Context.Entry(DBUserSettings).State = EntityState.Detached;
                Context.Entry(userSettings).State = EntityState.Modified;
                if (Context.SaveChanges() > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool ChangePassword(ChangePassword changePassword, int userId)
        {
            User dbUser = Get(userId);
            if (dbUser != null && dbUser.Password == Hashing.Hash(changePassword.OldPassword, dbUser.Salt) && changePassword.NewPassword == changePassword.ConfirmNewPassword)
            {
                dbUser.Salt = Hashing.GetSalt();
                dbUser.Password = Hashing.Hash(changePassword.NewPassword, dbUser.Salt);
                Context.Entry(dbUser).State = EntityState.Modified;
                if (Context.SaveChanges() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<UserProfile> Search(string fullName)
        {
            String FullName = fullName.ToLower();
            return Context.Users.Where(x => (x.FirstName + " " + x.LastName).ToLower().Contains(FullName) && x.RecordStatusId == (int)RecordStatus.Active)
                    .Select(x => new UserProfile { FirstName = x.FirstName, LastName = x.LastName, UserId = x.UserId });
        }

        public UserProfile SetAdditionalSettingsForProfile(UserProfile profile, int loginUserId)
        {
            UserFriend Friend = Context.UserFriends.Where(x =>
                                (x.IdUser == profile.UserId && x.IdUserToFriendList == loginUserId) ||
                                (x.IdUser == loginUserId && x.IdUserToFriendList == profile.UserId))
                                .FirstOrDefault();
            UserBlacklist BlackList = Context.UserBlacklists.Where(x =>
                                (x.IdUser == profile.UserId && x.IdUserToBlackList == loginUserId) ||
                                (x.IdUser == loginUserId && x.IdUserToBlackList == profile.UserId))
                                .FirstOrDefault();

            if (Friend != null)
            {
                if (Friend.RequestAccepted)
                {
                    profile.AreFriends = true;
                }
                else
                {
                    profile.FriendRequestSend = true;
                }
            }

            if (BlackList != null)
            {
                 profile.IsBlocked = false;
            }

            if (Context.UsersFollowings.Any(x=>x.IdUser == loginUserId && x.IdUserToFollow == profile.UserId))
            {
                profile.IsFollowing = true;
            }

            return profile;
        }

        public bool IsOnFriendList(int userId, int loginUserId)
        {
            return Context.UserFriends.Any(x =>
                                (x.IdUser == userId && x.IdUserToFriendList == loginUserId) ||
                                (x.IdUser == loginUserId && x.IdUserToFriendList == userId));
        }

        public bool IsOnBlockList(int userId, int loginUserId)
        {
            return Context.UserBlacklists.Any(x =>
                                (x.IdUser == userId && x.IdUserToBlackList == loginUserId) ||
                                (x.IdUser == loginUserId && x.IdUserToBlackList == userId));
        }

        public bool SendFriendRequest(int userId, int loginUserId)
        {
            Context.UserFriends.Add(new UserFriend { IdUser = loginUserId, IdUserToFriendList = userId, RequestAccepted = false });
            return SaveChanges();
        }

        

        public bool RemoveFriend(int userId, int loginUserId)
        {
            Context.UserFriends.Remove(Context.UserFriends.FirstOrDefault(x=>x.IdUser == loginUserId && x.IdUserToFriendList == userId || x.IdUser == userId && x.IdUserToFriendList == loginUserId));
            return SaveChanges();
        }

        public bool BlockUser(int userId, int loginUserId)
        {
            Context.UserBlacklists.Add(new UserBlacklist { IdUser = loginUserId, IdUserToBlackList = userId });
            return SaveChanges();
        }

        public bool UnblockUser(int userId, int loginUserId)
        {
            Context.UserBlacklists.Remove(Context.UserBlacklists.FirstOrDefault(x => x.IdUser == loginUserId && x.IdUserToBlackList == userId || x.IdUser == userId && x.IdUserToBlackList == loginUserId));
            return SaveChanges();
        }

        public bool FollowUser(int userId, int loginUserId)
        {
            Context.UsersFollowings.Add(new UserFollow { IdUser = loginUserId, IdUserToFollow = userId });
            return SaveChanges();
        }

        public bool CanFollow(int userId, int loginUserId)
        {
            if (IsOnBlockList(userId,loginUserId) == false && Context.UserSettings.FirstOrDefault(x=>x.IdUser == userId).AllowFollowing && IsFolowing(userId, loginUserId) == false)
            {
                return true;
            }

            return false;
        }

        public bool IsFolowing(int userId, int loginUserId)
        {
            return Context.UsersFollowings.Any(x =>
                                 (x.IdUser == userId && x.IdUserToFollow == loginUserId) ||
                                 (x.IdUser == loginUserId && x.IdUserToFollow == userId));
        }

        public bool StopFollowingUser(int userId, int loginUserId)
        {
            Context.UsersFollowings.Remove(Context.UsersFollowings.FirstOrDefault(x => x.IdUser == loginUserId && x.IdUserToFollow == userId || x.IdUser == userId && x.IdUserToFollow == loginUserId));
            return SaveChanges();
        }

        public IEnumerable<UserFriend> GetFriends(int userId)
        {
            List<UserFriend> UserFriends = new List<UserFriend>();

           var List = (from uf in Context.UserFriends
             join Sender in Context.Users on uf.IdUser equals Sender.UserId
             join Reciver in Context.Users on uf.IdUserToFriendList equals Reciver.UserId
             where (uf.IdUser == userId || uf.IdUserToFriendList == userId) && Sender.RecordStatusId == (int)RecordStatus.Active && Reciver.RecordStatusId == (int)RecordStatus.Active
             select new { UserFriend = uf, UserSender = Sender, UserReciver = Reciver }).ToList();

            foreach (var item in List)
            {
                if (item.UserSender.UserId != userId)
                {
                    item.UserFriend.Friend = item.UserSender;
                }
                else
                {
                    item.UserFriend.Friend = item.UserReciver;
                    item.UserFriend.RequestSent = true;
                }
                UserFriends.Add(item.UserFriend);
            }

            return UserFriends;
        }

        public bool ConfirmFriendRequest(int userFriendId, int userId)
        {
            UserFriend ConfirmFriendRequest = Context.UserFriends.FirstOrDefault(x=>x.UserFriendId == userFriendId && x.RequestAccepted == false);
            if (ConfirmFriendRequest != null && ConfirmFriendRequest.IdUserToFriendList == userId)
            {
                ConfirmFriendRequest.RequestAccepted = true;
                Context.Entry(ConfirmFriendRequest).State = EntityState.Modified;
                return SaveChanges();
            }
            return false;
        }

        public bool DenyFriendRequest(int userFriendId, int userId)
        {
            UserFriend ConfirmFriendRequest = Context.UserFriends.FirstOrDefault(x => x.UserFriendId == userFriendId && x.RequestAccepted == false);
            if (ConfirmFriendRequest != null && ConfirmFriendRequest.IdUserToFriendList == userId)
            {
                Context.UserFriends.Remove(ConfirmFriendRequest);
                return SaveChanges();
            }
            return false;
        }
    }
}