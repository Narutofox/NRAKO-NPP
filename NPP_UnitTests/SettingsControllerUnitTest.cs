using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NRAKO_IvanCicek.Controllers;
using NRAKO_IvanCicek.Helpers;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;
using NRAKO_IvanCicek.Models.VM;

namespace NPP_UnitTests
{
    [TestClass]
    public class SettingsControllerUnitTest
    {
        private LoginUser _loginUser;
        private Mock<IUserDAL> _userRepoMock;

        [TestInitialize]
        public void Initialize()
        {
            _loginUser = Helper.GetLoginUser();
            _userRepoMock = new Mock<IUserDAL>();
        }

        [TestMethod]
        public void Index()
        {
            _userRepoMock.Setup(m => m.Get( _loginUser.UserId)).Returns(new User());
            _userRepoMock.Setup(m => m.GetUserSettings(_loginUser.UserId)).Returns(new UserSetting());
            SettingsController controller = new SettingsController(_userRepoMock.Object, _loginUser);
            ViewResult result = controller.Index();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "");
            Assert.IsNotNull(result.Model);
            Assert.IsTrue(result.Model is User);
            Assert.IsNotNull(result.ViewBag.UserSettings);
            Assert.IsTrue(result.ViewBag.UserSettings is UserSetting);
        }

        [TestMethod]
        public void ChangePasswordGet()
        {
            SettingsController controller = new SettingsController(_userRepoMock.Object, _loginUser);
            ViewResult result = controller.ChangePassword();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "");
            Assert.IsNull(result.Model);            
        }

        [TestMethod]
        public void ChangePasswordPost()
        {
            ChangePassword model = new ChangePassword();;
            _userRepoMock.Setup(m => m.ChangePassword(model, _loginUser.UserId)).Returns(true);
            SettingsController controller = new SettingsController(_userRepoMock.Object, _loginUser);
            ActionResult result = controller.ChangePassword(model);

            Assert.IsNotNull(result);
            Assert.IsTrue(result is RedirectToRouteResult);
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("action"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("controller"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["action"].ToString() == "Index");
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["controller"].ToString() == "Home");

            Assert.IsTrue(controller.TempData.ContainsKey("notification"));
            Assert.IsTrue(controller.TempData["notification"] is Notification);
            Assert.IsTrue((controller.TempData["notification"] as Notification).Type == NotificationType.Success);
            Assert.IsTrue(!String.IsNullOrEmpty((controller.TempData["notification"] as Notification).Text));
        }

        [TestMethod]
        public void ChangePasswordPost_Failed()
        {
            ChangePassword model = new ChangePassword();
            _userRepoMock.Setup(m => m.ChangePassword(model, _loginUser.UserId)).Returns(false);
            SettingsController controller = new SettingsController(_userRepoMock.Object, _loginUser);
            ActionResult result = controller.ChangePassword(model);

            Assert.IsNotNull(result);
            Assert.IsTrue(result is ViewResult);
            Assert.IsTrue((result as ViewResult).ViewName == "");
            Assert.IsNull((result as ViewResult).Model);
            Assert.IsTrue(controller.TempData.ContainsKey("notification"));
            Assert.IsTrue(controller.TempData["notification"] is Notification);
            Assert.IsTrue((controller.TempData["notification"] as Notification).Type == NotificationType.Error);
            Assert.IsTrue(!String.IsNullOrEmpty((controller.TempData["notification"] as Notification).Text));
        }

        [TestMethod]
        public void UpdateData_ImageTypeAllowed()
        {
            
            User user = new User(){UserId = -5};
            UserSetting userSettings = new UserSetting(){IdUser = -111};

            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
            Mock<HttpPostedFileBase> file = new Mock<HttpPostedFileBase>();

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello"));

            file.Setup(x => x.InputStream).Returns(stream);
            file.Setup(x => x.ContentLength).Returns((int)stream.Length);
            file.Setup(x => x.FileName).Returns("Test.js");
            string basePath = @"c:\yourPath\App";

            contextMock.Setup(x => x.Server.MapPath(It.IsAny<String>())).Returns(basePath);
            file.Setup(x => x.SaveAs(It.IsAny<String>())).Verifiable();


            string folderPath = basePath + HardcodedValues.UserFiles + _loginUser.UserId;
            var fakeFileSystem =
                new MockFileSystem(new Dictionary<string, MockFileData> { { folderPath, new MockDirectoryData() } });

            SettingsController controller = new SettingsController(_userRepoMock.Object, _loginUser, fakeFileSystem);
            RequestContext rc = new RequestContext(contextMock.Object, new RouteData());
            controller.ControllerContext = new ControllerContext(rc, controller);

            ActionResult result = controller.UpdateData(user, userSettings, file.Object);

            Assert.IsNotNull(result);
            Assert.IsTrue(result is ViewResult);
            Assert.IsTrue((result as ViewResult).ViewName == "Index");
            Assert.IsNotNull((result as ViewResult).Model);
            Assert.IsTrue((result as ViewResult).Model is User);
            Assert.IsNotNull(controller.ViewBag.UserSettings);
            Assert.IsTrue(controller.TempData.ContainsKey("notification"));
            Assert.IsTrue(controller.TempData["notification"] is Notification);
            Assert.IsTrue((controller.TempData["notification"] as Notification).Type == NotificationType.Error);
        }

        [TestMethod]
        public void UpdateData_UpdateFailed()
        {

            User user = new User() { UserId = -5 };
            UserSetting userSettings = new UserSetting() { IdUser = -111 };

            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
            Mock<HttpPostedFileBase> file = new Mock<HttpPostedFileBase>();

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello"));

            file.Setup(x => x.InputStream).Returns(stream);
            file.Setup(x => x.ContentLength).Returns((int)stream.Length);
            file.Setup(x => x.FileName).Returns("Test.jpg");
            string basePath = @"c:\yourPath\App";

            contextMock.Setup(x => x.Server.MapPath(It.IsAny<String>())).Returns(basePath);
            file.Setup(x => x.SaveAs(It.IsAny<String>())).Verifiable();
            _userRepoMock.Setup(m => m.Update(user)).Returns(false);

            string folderPath = basePath + HardcodedValues.UserFiles + _loginUser.UserId;
            var fakeFileSystem =
                new MockFileSystem(new Dictionary<string, MockFileData> { { folderPath, new MockDirectoryData() } });

            SettingsController controller = new SettingsController(_userRepoMock.Object, _loginUser, fakeFileSystem);
            RequestContext rc = new RequestContext(contextMock.Object, new RouteData());
            controller.ControllerContext = new ControllerContext(rc, controller);

            ActionResult result = controller.UpdateData(user, userSettings, file.Object);

            Assert.IsNotNull(result);
            Assert.IsTrue(result is ViewResult);
            Assert.IsTrue((result as ViewResult).ViewName == "Index");
            Assert.IsNotNull((result as ViewResult).Model);
            Assert.IsTrue((result as ViewResult).Model is User);
            Assert.IsNotNull(controller.ViewBag.UserSettings);
            Assert.IsTrue(controller.TempData.ContainsKey("notification"));
            Assert.IsTrue(controller.TempData["notification"] is Notification);
            Assert.IsTrue((controller.TempData["notification"] as Notification).Type == NotificationType.Error);
        }

        [TestMethod]
        public void UpdateData_UpdateUserSettingsFailed()
        {

            User user = new User() { UserId = -5 };
            UserSetting userSettings = new UserSetting() { IdUser = -111 };

            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
            Mock<HttpPostedFileBase> file = new Mock<HttpPostedFileBase>();

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello"));

            file.Setup(x => x.InputStream).Returns(stream);
            file.Setup(x => x.ContentLength).Returns((int)stream.Length);
            file.Setup(x => x.FileName).Returns("Test.jpg");
            string basePath = @"c:\yourPath\App";

            contextMock.Setup(x => x.Server.MapPath(It.IsAny<String>())).Returns(basePath);
            file.Setup(x => x.SaveAs(It.IsAny<String>())).Verifiable();
            _userRepoMock.Setup(m => m.Update(user)).Returns(true);
            _userRepoMock.Setup(m => m.UpdateUserSettings(userSettings)).Returns(false);

            string folderPath = basePath + HardcodedValues.UserFiles + _loginUser.UserId;
            var fakeFileSystem =
                new MockFileSystem(new Dictionary<string, MockFileData> { { folderPath, new MockDirectoryData() } });

            SettingsController controller = new SettingsController(_userRepoMock.Object, _loginUser, fakeFileSystem);
            RequestContext rc = new RequestContext(contextMock.Object, new RouteData());
            controller.ControllerContext = new ControllerContext(rc, controller);

            ActionResult result = controller.UpdateData(user, userSettings, file.Object);

            Assert.IsNotNull(result);
            Assert.IsTrue(result is ViewResult);
            Assert.IsTrue((result as ViewResult).ViewName == "Index");
            Assert.IsNotNull((result as ViewResult).Model);
            Assert.IsTrue((result as ViewResult).Model is User);
            Assert.IsNotNull(controller.ViewBag.UserSettings);
            Assert.IsTrue(controller.TempData.ContainsKey("notification"));
            Assert.IsTrue(controller.TempData["notification"] is Notification);
            Assert.IsTrue((controller.TempData["notification"] as Notification).Type == NotificationType.Error);
        }

        [TestMethod]
        public void UpdateData()
        {

            User user = new User() { UserId = -5 };
            UserSetting userSettings = new UserSetting() { IdUser = -111 };

            Mock<HttpContextBase> contextMock = new Mock<HttpContextBase>();
            Mock<HttpPostedFileBase> file = new Mock<HttpPostedFileBase>();

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello"));

            file.Setup(x => x.InputStream).Returns(stream);
            file.Setup(x => x.ContentLength).Returns((int)stream.Length);
            file.Setup(x => x.FileName).Returns("Test.jpg");
            string basePath = @"c:\yourPath\App";

            contextMock.Setup(x => x.Server.MapPath(It.IsAny<String>())).Returns(basePath);
            file.Setup(x => x.SaveAs(It.IsAny<String>())).Verifiable();
            _userRepoMock.Setup(m => m.Update(user)).Returns(true);
            _userRepoMock.Setup(m => m.UpdateUserSettings(userSettings)).Returns(true);

            string folderPath = basePath + HardcodedValues.UserFiles + _loginUser.UserId;
            var fakeFileSystem =
                new MockFileSystem(new Dictionary<string, MockFileData> { { folderPath, new MockDirectoryData() } });

            SettingsController controller = new SettingsController(_userRepoMock.Object, _loginUser, fakeFileSystem);
            RequestContext rc = new RequestContext(contextMock.Object, new RouteData());
            controller.ControllerContext = new ControllerContext(rc, controller);

            ActionResult result = controller.UpdateData(user, userSettings, file.Object);

            Assert.IsNotNull(result);
            Assert.IsTrue(result is RedirectToRouteResult);
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("action"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("controller"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["action"].ToString() == "Index");
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["controller"].ToString() == "Home");
            Assert.IsNull(controller.ViewBag.UserSettings);
            Assert.IsTrue(controller.TempData.ContainsKey("notification"));
            Assert.IsTrue(controller.TempData["notification"] is Notification);
            Assert.IsTrue((controller.TempData["notification"] as Notification).Type == NotificationType.Success);
        }
    }
}
