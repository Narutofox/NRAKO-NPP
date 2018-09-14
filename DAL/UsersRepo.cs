using NRAKO_IvanCicek.Helpers;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace NRAKO_IvanCicek.DAL
{
    public class UsersRepo : IUserDAL
    {
        private readonly Context _context;
        private UsersRepo(Context context = null)
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

        public static UsersRepo GetInstance(Context context = null)
        {
            return new UsersRepo(context);
        }

        private bool SaveChanges()
        {
            if (_context.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        public bool Create(User user)
        {
            user.RecordStatusId = (int)RecordStatus.Active;
            _context.Users.Add(user);
            _context.SaveChanges();
            _context.UserSettings.Add(new UserSetting { IdUser = user.UserId, AllowFollowing = false, ShowEmail = false });
            if (_context.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        public bool Update(User user)
        {
            User dbUser = Get(user.UserId);
            if (dbUser != null)
            {
                // Ovdje se lozinka i Salt NE mijenja
                user.Password = dbUser.Password;
                user.Salt = dbUser.Salt;
                // Ako je polje za putanju do slike profila prazno onda postavi onu vrijednost koja je u bazi
                if (String.IsNullOrEmpty(user.ProfileImagePath))
                {
                    user.ProfileImagePath = dbUser.ProfileImagePath;
                }

                if (user.RecordStatusId <= 0)
                {
                    user.RecordStatusId = (int)RecordStatus.Active;
                }

                _context.Entry(dbUser).State = EntityState.Detached;
                _context.Entry(user).State = EntityState.Modified;
                if (_context.SaveChanges() > 0)
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
            return _context.UserSettings.FirstOrDefault(x => x.IdUser == userId);
        }

        public User Get(int userId)
        {
            return _context.Users.FirstOrDefault(x => x.UserId == userId && x.RecordStatusId == (int)RecordStatus.Active);
        }

        public UserProfile GetProfileData(int userId)
        {
            User user = _context.Users.FirstOrDefault(x => x.UserId == userId && x.RecordStatusId == (int)RecordStatus.Active);

            if (user != null)
            {
                UserProfile profile = new UserProfile
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ProfileImagePath = user.ProfileImagePath
                };

                UserSetting settings = GetUserSettings(userId);

                profile.AllowFollowing = settings.AllowFollowing;
                profile.ShowEmail = settings.AllowFollowing;
                return profile;
            }
            return null;
        }

        public bool UpdateUserSettings(UserSetting userSettings)
        {
            UserSetting dbUserSettings = GetUserSettings(userSettings.IdUser);

            if (dbUserSettings != null)
            {
                userSettings.UserSettingId = dbUserSettings.UserSettingId;

                _context.Entry(dbUserSettings).State = EntityState.Detached;
                _context.Entry(userSettings).State = EntityState.Modified;
                if (_context.SaveChanges() > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public bool ChangePassword(ChangePassword changePassword, int userId)
        {
            User dbUser = Get(userId);
            if (dbUser != null && dbUser.Password == Hashing.Hash(changePassword.OldPassword, dbUser.Salt) 
                               && changePassword.NewPassword == changePassword.ConfirmNewPassword)
            {
                dbUser.Salt = Hashing.GetSalt();
                dbUser.Password = Hashing.Hash(changePassword.NewPassword, dbUser.Salt);
                _context.Entry(dbUser).State = EntityState.Modified;
                if (_context.SaveChanges() > 0)
                {
                    return true;
                }
            }
            return false;
        }

        public IEnumerable<UserProfile> Search(string fullName)
        {
            String fullNameLower = fullName.ToLower();
            return _context.Users.Where(x => (x.FirstName + " " + x.LastName).ToLower().Contains(fullNameLower) 
                                             && x.RecordStatusId == (int)RecordStatus.Active)
                    .Select(x => new UserProfile { FirstName = x.FirstName, LastName = x.LastName, UserId = x.UserId });
        }

        public UserProfile SetAdditionalSettingsForProfile(UserProfile profile, int loginUserId)
        {
            UserFriend friend = _context.UserFriends.FirstOrDefault(x =>
                                (x.IdUser == profile.UserId && x.IdUserToFriendList == loginUserId) ||
                                (x.IdUser == loginUserId && x.IdUserToFriendList == profile.UserId)) ;
            UserBlacklist blackList = _context.UserBlacklists.FirstOrDefault(x =>
                                (x.IdUser == profile.UserId && x.IdUserToBlackList == loginUserId) ||
                                (x.IdUser == loginUserId && x.IdUserToBlackList == profile.UserId));

            if (friend != null)
            {
                if (friend.RequestAccepted)
                {
                    profile.AreFriends = true;
                }
                else
                {
                    profile.FriendRequestSend = true;
                }
            }

            if (blackList != null)
            {
                 profile.IsBlocked = false;
            }

            if (_context.UsersFollowings.Any(x=>x.IdUser == loginUserId && x.IdUserToFollow == profile.UserId))
            {
                profile.IsFollowing = true;
            }

            return profile;
        }

        public bool IsOnFriendList(int userId, int loginUserId)
        {
            return _context.UserFriends.Any(x =>
                                (x.IdUser == userId && x.IdUserToFriendList == loginUserId) ||
                                (x.IdUser == loginUserId && x.IdUserToFriendList == userId));
        }

        public bool IsOnBlockList(int userId, int loginUserId)
        {
            bool isOnBlockList = _context.UserBlacklists.Any(x =>
                                (x.IdUser == userId && x.IdUserToBlackList == loginUserId) ||
                                (x.IdUser == loginUserId && x.IdUserToBlackList == userId));
            return isOnBlockList;
        }

        public bool SendFriendRequest(int userId, int loginUserId)
        {
            _context.UserFriends.Add(new UserFriend
            {
                IdUser = loginUserId,
                IdUserToFriendList = userId,
                RequestAccepted = false
            });
            return SaveChanges();
        }

        

        public bool RemoveFriend(int userId, int loginUserId)
        {
            _context.UserFriends.Remove(_context.UserFriends.Single(x=>x.IdUser == loginUserId 
                                                                       && x.IdUserToFriendList == userId || x.IdUser == userId
                                                                       && x.IdUserToFriendList == loginUserId));
            return SaveChanges();
        }

        public bool BlockUser(int userId, int loginUserId)
        {
            _context.UserBlacklists.Add(new UserBlacklist { IdUser = loginUserId, IdUserToBlackList = userId });
            return SaveChanges();
        }

        public bool UnblockUser(int userId, int loginUserId)
        {
            _context.UserBlacklists.Remove(_context.UserBlacklists
                .First(x => x.IdUser == loginUserId && x.IdUserToBlackList == userId || x.IdUser == userId 
                            && x.IdUserToBlackList == loginUserId));
            return SaveChanges();
        }

        public bool FollowUser(int userId, int loginUserId)
        {
            _context.UsersFollowings.Add(new UserFollow { IdUser = loginUserId, IdUserToFollow = userId });
            return SaveChanges();
        }

        public bool CanFollow(int userId, int loginUserId)
        {
            bool allowFollowing = _context.UserSettings.First(x => x.IdUser == userId).AllowFollowing;
            if (!IsOnBlockList(userId, loginUserId) && allowFollowing && !IsFollowing(userId, loginUserId))
            {
                return true;
            }

            return false;
        }

        public bool IsFollowing(int userId, int loginUserId)
        {
            bool isFollowing = _context.UsersFollowings.Any(x =>
                                 (x.IdUser == userId && x.IdUserToFollow == loginUserId) ||
                                 (x.IdUser == loginUserId && x.IdUserToFollow == userId));
            return isFollowing;
        }

        public bool StopFollowingUser(int userId, int loginUserId)
        {
            _context.UsersFollowings.Remove(_context.UsersFollowings
                .Single(x => x.IdUser == loginUserId && x.IdUserToFollow == userId || x.IdUser == userId 
                             && x.IdUserToFollow == loginUserId));
            return SaveChanges();
        }

        public IEnumerable<UserFriend> GetFriends(int userId)
        {
            List<UserFriend> userFriends = new List<UserFriend>();

           var list = (from uf in _context.UserFriends
             join sender in _context.Users on uf.IdUser equals sender.UserId
             join reciver in _context.Users on uf.IdUserToFriendList equals reciver.UserId
             where (uf.IdUser == userId || uf.IdUserToFriendList == userId) 
                   && sender.RecordStatusId == (int)RecordStatus.Active && reciver.RecordStatusId == (int)RecordStatus.Active
             select new { UserFriend = uf, UserSender = sender, UserReciver = reciver }).ToList();

            foreach (var item in list)
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
                userFriends.Add(item.UserFriend);
            }

            return userFriends;
        }

        public bool ConfirmFriendRequest(int userFriendId, int userId)
        {
            UserFriend confirmFriendRequest = _context.UserFriends.FirstOrDefault(x=>
                x.UserFriendId == userFriendId && !x.RequestAccepted);
            if (confirmFriendRequest != null && confirmFriendRequest.IdUserToFriendList == userId)
            {
                confirmFriendRequest.RequestAccepted = true;
                _context.Entry(confirmFriendRequest).State = EntityState.Modified;
                return SaveChanges();
            }
            return false;
        }

        public bool DenyFriendRequest(int userFriendId, int userId)
        {
            UserFriend confirmFriendRequest = _context.UserFriends.FirstOrDefault(x => 
                x.UserFriendId == userFriendId && !x.RequestAccepted);
            if (confirmFriendRequest != null && confirmFriendRequest.IdUserToFriendList == userId)
            {
                _context.UserFriends.Remove(confirmFriendRequest);
                return SaveChanges();
            }
            return false;
        }
    }
}