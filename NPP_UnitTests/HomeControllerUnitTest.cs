using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRAKO_IvanCicek.Controllers;
using System.Web.Mvc;
using Moq;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models.VM;

namespace NPP_UnitTests
{
    [TestClass]
    public class HomeControllerUnitTest
    {
        private Mock<IPostsRepo> _postsRepoMock;

        [TestInitialize]
        public void Initialize()
        {
            _postsRepoMock = new Mock<IPostsRepo>();
            _postsRepoMock.Setup(m => m.GetVisibilityOptions()).Returns(new List<EnumVM>());
        }

        [TestMethod]
        public void Index()
        {
            HomeController controller = new HomeController(_postsRepoMock.Object);
            ViewResult result = controller.Index();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "");
            Assert.IsNull(result.Model);
            Assert.IsNotNull(controller.ViewBag.VisibilityOptions);
            Assert.IsTrue(controller.ViewBag.VisibilityOptions is List<EnumVM>);
        }
    }
}
