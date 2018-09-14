using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRAKO_IvanCicek.Controllers;
using System.Web.Mvc;

namespace NPP_UnitTests
{
    [TestClass]
    public class HomeControllerUnitTest
    {

        private HomeController _controller;
        [TestInitialize]
        public void Initialize()
        {
            _controller = new HomeController(Helper.GetContext());
        }

        [TestMethod]
        public void Index()
        {
            ViewResult result = _controller.Index();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "");
            Assert.IsNull(result.Model);
            Assert.IsNotNull(_controller.ViewBag.VisibilityOptions);           
        }
    }
}
