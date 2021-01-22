using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MSensis.Controllers;
using MSensis.Email;
using MSensis.Models;
using MSensis.ViewModels;
using MSensis.Services;
using Moq;


namespace MSensis.Tests
{
    [TestClass]
    public class UnitTest1
    {
        Mock<HomeController> controller = new Mock<HomeController>();

        [TestMethod]
        public void TestMethod1()
        {
            UserForProfileViewModel model = new UserForProfileViewModel
            {
                Name = "Giannis",
                PhoneNumber = "123"
            };
            var result = controller.Setup(x => x.UpdateProfile(model).Result);
            Assert.AreEqual("Giannis", "Giannis");

        }
    }
}
