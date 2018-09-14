using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRAKO_IvanCicek.Controllers;
using NRAKO_IvanCicek.Models.VM;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Moq;
using NRAKO_IvanCicek.Helpers;
using NRAKO_IvanCicek.Interfaces;
using NRAKO_IvanCicek.Models;

namespace NPP_UnitTests
{
    [TestClass]
    public class SignUpControllerUnitTest
    {
        private Mock<IUserDAL> _userRepoMock;

        [TestInitialize]
        public void Initialize()
        {
            _userRepoMock = new Mock<IUserDAL>();
        }

        [TestMethod]
        public void Index()
        {
            SignUpController controller = new SignUpController(_userRepoMock.Object);
            ViewResult result = controller.Index();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.ViewName == "");
        }

        [TestMethod]
        public void SignUp()
        {
            SignUpController controller = new SignUpController(_userRepoMock.Object);
            SignUp signUpUser = new SignUp();

            signUpUser.FirstName = "Naruto";
            signUpUser.LastName = "Uzumaki";
            signUpUser.Email = "sdfs@vd.voc";
            signUpUser.Password = "12345678";

            SetModelState(signUpUser, controller);
            _userRepoMock.Setup(m => m.Create(It.IsAny<User>())).Returns(true);

            ActionResult result = controller.SignUp(signUpUser);
            Assert.IsNotNull(result);
            Assert.IsTrue(result is RedirectToRouteResult);
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("action"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("controller"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["action"].ToString() == "Index");
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["controller"].ToString() == "Home");
        }

        [TestMethod]
        public void SignUp_CreateFailed()
        {
            SignUpController controller = new SignUpController(_userRepoMock.Object);
            SignUp signUpUser = new SignUp();

            signUpUser.FirstName = "Naruto";
            signUpUser.LastName = "Uzumaki";
            signUpUser.Email = "sdfs@vd.voc";
            signUpUser.Password = "12345678";

            SetModelState(signUpUser, controller);
            _userRepoMock.Setup(m => m.Create(It.IsAny<User>())).Returns(false);

            ActionResult result = controller.SignUp(signUpUser);
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ViewResult);
            Assert.IsTrue((result as ViewResult).ViewName == "Index");
            Assert.IsTrue(controller.TempData.ContainsKey("notification"));
            Assert.IsTrue(controller.TempData["notification"] is Notification);
            Assert.IsTrue((controller.TempData["notification"] as Notification).Type == NotificationType.Error);
        }

        [TestMethod]
        public void SignUp_ModelStateNotValid()
        {
            SignUpController controller = new SignUpController(_userRepoMock.Object);
            SignUp signUpUser = new SignUp();

            SetModelState(signUpUser, controller);

            ActionResult result = controller.SignUp(signUpUser);
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ViewResult);
            Assert.IsTrue((result as ViewResult).ViewName == "Index");
        }

        private void SetModelState(object model, Controller controller)
        {
            controller.ModelState.Clear();
            ValidationContext validationContext = new ValidationContext(model, null, null);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            foreach (var validationResult in validationResults)
            {
                controller.ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
            }
        }
    }
}
