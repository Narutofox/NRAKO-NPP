using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRAKO_IvanCicek.Controllers;

namespace NPP_UnitTests
{
    [TestClass]
    public class ErrorControllerUnitTests
    {
        [TestMethod]
        public void Index()
        {
            ViewResult result = new ErrorController().Index();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "Error");
            Assert.IsNull(result.Model);
        }
    }
}
