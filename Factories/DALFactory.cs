using NRAKO_IvanCicek.DAL;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NRAKO_IvanCicek.Factories
{
    public static class DalFactory
    {
        public static IUserDAL GetUsersRepo(Context context = null)
        {
            return UsersRepo.GetInstance(context);
        }

        public static ILoginDAL GetLoginRepo(Context context = null)
        {
            return LoginRepo.GetInstance(context);
        }

        public static IPostsRepo GetPostsRepo(Context context = null)
        {
            return PostsRepo.GetInstance(context);
        }
    }
}