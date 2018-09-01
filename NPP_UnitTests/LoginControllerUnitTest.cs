using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRAKO_IvanCicek.Controllers;
using NRAKO_IvanCicek.Models.VM;
using System.Web.Mvc;
using System.Web.Routing;
using NRAKO_IvanCicek.Factories;
using NRAKO_IvanCicek.Models;
using System.Data.SqlClient;

namespace NPP_UnitTests
{
    [TestClass]
    public class LoginControllerUnitTest
    {
        private LoginController _controller;
        private Context _context;
        [TestInitialize]
        public void Initialize()
        {
            _context = Helper.GetContext();
            _controller = new LoginController(_context);           
        }

        [TestMethod]
        public void Index()
        {
            ViewResult result = _controller.Index();
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ViewResult);
            Assert.IsTrue(result.ViewName == "");
        }

        [TestMethod]
        public void Login()
        {
            Login login = new Login();

            ActionResult result = _controller.Login(login);
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ViewResult);
            Assert.IsTrue((result as ViewResult).ViewName == "Index");
            Assert.IsTrue((result as ViewResult).Model is Login);
            Assert.IsTrue(_controller.TempData.ContainsKey("notification"));

            login.Email = "test2@nrako.com";
            login.Password = "1";
            result = _controller.Login(login);
            Assert.IsNotNull(result);
            Assert.IsTrue(result is RedirectToRouteResult);
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("action"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("controller"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["action"].ToString() == "Index");
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["controller"].ToString() == "Home");
        }

        [TestMethod]
        public void Logoff()
        {
            ActionResult result = _controller.Logoff();
            Assert.IsTrue(_controller.TempData.ContainsKey("notification"));
            Assert.IsNotNull(result);
            Assert.IsTrue(result is RedirectToRouteResult);
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("action"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("controller"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["action"].ToString() == "Index");
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["controller"].ToString() == "Home");
        }
    }
}
