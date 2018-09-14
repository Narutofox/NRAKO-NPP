using NRAKO_IvanCicek.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NRAKO_IvanCicek.Models.VM;
using System.Web.Mvc;

namespace NRAKO_IvanCicek.Interfaces
{
    public interface IUserDAL
    {
        bool Create(User user);
        bool Update(User user);
        bool Delete(User user);

        User Get(int userId);
        UserSetting GetUserSettings(int userId);
        bool UpdateUserSettings(UserSetting userSettings);
        bool ChangePassword(ChangePassword changePassword, int userId);
        UserProfile GetProfileData(int userId);
        IEnumerable<UserProfile> Search(string fullName);
        UserProfile SetAdditionalSettingsForProfile(UserProfile profile, int loginUserId);
        bool IsOnFriendList(int userId, int loginUserId);
        bool IsOnBlockList(int userId, int loginUserId);
        bool SendFriendRequest(int userId, int loginUserId);
        bool RemoveFriend(int userId, int loginUserId);
        bool BlockUser(int userId, int loginUserId);
        bool UnblockUser(int userId, int loginUserId);
        bool FollowUser(int userId, int loginUserId);
        bool CanFollow(int userId, int loginUserId);
        bool IsFollowing(int userId, int loginUserId);
        bool StopFollowingUser(int userId, int loginUserId);
        IEnumerable<UserFriend> GetFriends(int userId);
        bool ConfirmFriendRequest(int userFriendId, int userId);
        bool DenyFriendRequest(int userFriendId, int userId);
    }
}
