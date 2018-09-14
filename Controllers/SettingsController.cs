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
    public class SettingsController : Controller
    {
        IUserDAL userDAL;
        LoginUser LoginUser;
        private readonly IFileSystem _fileSystem;
        public SettingsController()
        {
            userDAL = DALFactory.GetUserDAL();
            LoginUser = (LoginUser)MySession.Get("LoginUser");
        }

        public SettingsController(IUserDAL userRepo, LoginUser loginUser, IFileSystem fileSystem = null)
        {
            userDAL = userRepo;
            LoginUser = loginUser;
            _fileSystem = fileSystem ?? new FileSystem();
        }

        // GET: Settings
        public ViewResult Index()
        {
            User user = userDAL.Get(LoginUser.UserId);
            ViewBag.UserSettings = userDAL.GetUserSettings(LoginUser.UserId);
            return View(user);
        }

        public ViewResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePassword changePassword)
        {
            Notification notification = new Notification();
            if (userDAL.ChangePassword(changePassword, LoginUser.UserId))
            {
                notification.Type = NotificationType.Success;
                notification.Text = "Lozinka uspješno spremljeni";
            }
            else
            {
                notification.Type = NotificationType.Error;
                notification.Text = "Dogodila se pogreška tokom uređenja lozinke";
            }

            TempData["notification"] = notification;
            if (notification.Type == NotificationType.Error)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }          
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateData(User user, UserSetting userSettings, HttpPostedFileBase profileImage)
        {
            Notification notification = new Notification();

            user.UserId = LoginUser.UserId;
            userSettings.IdUser = LoginUser.UserId;

            bool Error = false;
            
            if (profileImage != null)
            {
               string FileType = Path.GetExtension(profileImage.FileName);
                if (Helper.CheckImageTypeAllowed(FileType))
                {
                    user.ProfileImagePath = "~/Content/UserProfileImages/" + LoginUser.UserId + "/" + profileImage.FileName;
                    string folderPath = Server.MapPath("~/Content/UserProfileImages/" + LoginUser.UserId);
                    if (!_fileSystem.Directory.Exists(folderPath))
                    {
                        _fileSystem.Directory.CreateDirectory(folderPath);
                    }
                }
                else
                {
                    notification.Type = NotificationType.Error;
                    notification.Text = "Slika nije u dozvoljenom formatu";
                    Error = true;
                }
            }

            if (Error == false)
            {
                if (userDAL.Update(user))
                {
                    if (userDAL.UpdateUserSettings(userSettings))
                    {
                        if (!String.IsNullOrEmpty(user.ProfileImagePath) && profileImage != null)
                        {
                            profileImage.SaveAs(Server.MapPath(user.ProfileImagePath));
                        }

                        LoginUser.FirstName = user.FirstName;
                        LoginUser.LastName = user.LastName;
                        LoginUser.Email = user.Email;

                        notification.Type = NotificationType.Success;
                        notification.Text = "Podaci uspješno spremljeni";
                    }
                    else
                    {
                        notification.Type = NotificationType.Error;
                        notification.Text = "Dogodila se pogreška tokom uređenja postavki";
                    }
                }
                else
                {
                    notification.Type = NotificationType.Error;
                    notification.Text = "Dogodila se pogreška tokom uređenja osobnih podataka";
                }
            }
            

            TempData["notification"] = notification;
            if (notification.Type == NotificationType.Error)
            {
                ViewBag.UserSettings = userSettings;
                return View("Index", user);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }          
        }
    }
}