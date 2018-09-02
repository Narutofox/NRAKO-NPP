using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRAKO_IvanCicek.Controllers;
using NRAKO_IvanCicek.Helpers;
using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;

namespace NPP_UnitTests
{
    [TestClass]
    public class SettingsControllerUnitTest
    {
        private SettingsController _controller;
        [TestInitialize]
        public void Initialize()
        {
            _controller = new SettingsController(Helper.GetContext(),Helper.GetLoginUser());
        }

        [TestMethod]
        public void Index()
        {
            ViewResult result = _controller.Index();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "");
            Assert.IsNotNull(result.Model);
            Assert.IsTrue(result.Model is User);
            Assert.IsNotNull(_controller.ViewBag.UserSettings);
            Assert.IsTrue(_controller.ViewBag.UserSettings is UserSetting);
        }

        [TestMethod]
        public void ChangePasswordGet()
        {
            ViewResult result = _controller.ChangePassword();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "");
            Assert.IsNull(result.Model);            
        }

        [TestMethod]
        public void ChangePasswordPost()
        {
            ChangePassword changePassword = new ChangePassword(){NewPassword = "5",ConfirmNewPassword = "fggdf",OldPassword = ""};
            ActionResult result = _controller.ChangePassword(changePassword);
            
            Assert.IsNotNull(result);
            Assert.IsTrue(_controller.TempData.ContainsKey("notification"));
            Assert.IsTrue(_controller.TempData["notification"] is Notification);
            Assert.IsTrue((_controller.TempData["notification"] as Notification).Type == NotificationType.Error, "OldPassword mora biti točan");
            Assert.IsTrue((result as ViewResult).ViewName == "");
            Assert.IsNull((result as ViewResult).Model);

            changePassword = new ChangePassword() { NewPassword = "5", ConfirmNewPassword = "fggdf", OldPassword = "1"};
            result = _controller.ChangePassword(changePassword);

            Assert.IsNotNull(result);
            Assert.IsTrue(_controller.TempData.ContainsKey("notification"));
            Assert.IsTrue(_controller.TempData["notification"] is Notification);
            Assert.IsTrue((_controller.TempData["notification"] as Notification).Type == NotificationType.Error, "NewPassword i ConfirmNewPassword moraju biti isti ");
            Assert.IsTrue((result as ViewResult).ViewName == "");
            Assert.IsNull((result as ViewResult).Model);

            changePassword = new ChangePassword() { NewPassword = "5", ConfirmNewPassword = "5", OldPassword = "1" };
            result = _controller.ChangePassword(changePassword);

            Assert.IsNotNull(result);
            Assert.IsTrue(_controller.TempData.ContainsKey("notification"));
            Assert.IsTrue(_controller.TempData["notification"] is Notification);
            Assert.IsTrue((_controller.TempData["notification"] as Notification).Type == NotificationType.Success);
            Assert.IsTrue(result is RedirectToRouteResult);
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("action"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("controller"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["action"].ToString() == "Index");
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["controller"].ToString() == "Home");
        }
    }
}
