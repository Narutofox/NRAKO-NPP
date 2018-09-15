using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRAKO_IvanCicek.Controllers;
using NRAKO_IvanCicek.Models.VM;
using System.Web.Mvc;
using Moq;
using NRAKO_IvanCicek.Helpers;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;

namespace NPP_UnitTests
{
    [TestClass]
    public class LoginControllerUnitTest
    {
        private Mock<ILoginDAL> _loginRepoMock;

        [TestInitialize]
        public void Initialize()
        {
            _loginRepoMock = new Mock<ILoginDAL>();
        }

        [TestMethod]
        public void Index()
        {
            LoginController controller = new LoginController(_loginRepoMock.Object);

            ViewResult result = controller.Index();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "");
        }

        [TestMethod]
        public void Login()
        {
            Login login = new Login();
            LoginController controller = new LoginController(_loginRepoMock.Object);
            _loginRepoMock.Setup(x => x.LoginCheck(login)).Returns(new LoginUser());

            ActionResult result = controller.Login(login);
            Assert.IsNotNull(result);
            Assert.IsTrue(result is RedirectToRouteResult);
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("action"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("controller"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["action"].ToString() == "Index");
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["controller"].ToString() == "Home");
        }

        [TestMethod]
        public void Login_Failed()
        {
            Login login = new Login();
            LoginController controller = new LoginController(_loginRepoMock.Object);
            _loginRepoMock.Setup(x => x.LoginCheck(login)).Returns((LoginUser)null);

            ActionResult result = controller.Login(login);
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ViewResult);
            Assert.IsTrue((result as ViewResult).ViewName == "Index");
            Assert.IsTrue((result as ViewResult).Model is Login);
            Assert.IsTrue(controller.TempData.ContainsKey("notification"));
            Assert.IsTrue(controller.TempData["notification"] is Notification);
            Assert.IsTrue((controller.TempData["notification"] as Notification).Type == NotificationType.Error);
            Assert.IsTrue(!String.IsNullOrEmpty((controller.TempData["notification"] as Notification).Text));
        }

        [TestMethod]
        public void Logoff()
        {
            LoginController controller = new LoginController(_loginRepoMock.Object);
            ActionResult result = controller.Logoff();
            Assert.IsTrue(controller.TempData.ContainsKey("notification"));
            Assert.IsNotNull(result);
            Assert.IsTrue(result is RedirectToRouteResult);
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("action"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("controller"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["action"].ToString() == "Index");
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["controller"].ToString() == "Home");
        }
    }
}
