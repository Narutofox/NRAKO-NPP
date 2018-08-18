using NRAKO_IvanCicek.DAL;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NRAKO_IvanCicek.Factories
{
    public static class DALFactory
    {
        public static IUserDAL GetUserDAL(Context context = null)
        {
            return UsersDAL.GetInstance(context);
        }

        public static ILoginDAL GetLoginDAL(Context context = null)
        {
            return LoginDAL.GetInstance(context);
        }

        public static IPostsRepo GetPostDAL(Context context = null)
        {
            return PostsRepo.GetInstance(context);
        }
    }
}