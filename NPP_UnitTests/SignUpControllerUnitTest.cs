using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NRAKO_IvanCicek.Controllers;
using NRAKO_IvanCicek.Models.VM;

namespace NPP_UnitTests
{
    [TestClass]
    public class SignUpControllerUnitTest
    {
        private SignUpController _controller;
        [TestInitialize]
        public void Initialize()
        {
            _controller = new SignUpController(Helper.GetContext());
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
        public void SignUp()
        {
            SignUp signUpUser = new SignUp();

            SetModelState(signUpUser);

            ActionResult result = _controller.SignUp(signUpUser);
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ViewResult);
            Assert.IsTrue((result as ViewResult).ViewName == "Index");

            signUpUser.FirstName = "Naruto";
            signUpUser.LastName = "Uzumaki";
            signUpUser.Email = "sdfs@vd.voc";
            signUpUser.Password = "12345678";

            SetModelState(signUpUser);

            result = _controller.SignUp(signUpUser);
            Assert.IsNotNull(result);
            Assert.IsTrue(result is RedirectToRouteResult);
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("action"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues.ContainsKey("controller"));
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["action"].ToString() == "Index");
            Assert.IsTrue((result as RedirectToRouteResult).RouteValues["controller"].ToString() == "Home");
        }

        private void SetModelState(object model)
        {
            _controller.ModelState.Clear();
            ValidationContext validationContext = new ValidationContext(model, null, null);
            List<ValidationResult> validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            foreach (var validationResult in validationResults)
            {
                _controller.ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
            }
        }
    }
}
