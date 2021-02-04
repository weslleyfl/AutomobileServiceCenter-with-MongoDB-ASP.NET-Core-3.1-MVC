using ASC.Web.Controllers;
using System;
using Xunit;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using ASC.Web.Models;
using ASC.Web.Configuration;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ASC.Tests.TestUtilities;
using ASC.Utilities;
using ASC.Business.Interfaces;

namespace ASC.Tests
{
    public class HomeControllerTests
    {
        /*
        ///  we understand that the home page should
        ///  return a type of ViewResult, 
        ///  should not return any model, and 
        ///  should contain no validation errors.
        ///  The naming convention is self-explanatory: ControllerName_ActionName_TestCondition_Test.
        */

        private readonly Mock<IOptions<ApplicationSettings>> _optionsMock;
        private readonly ILogger<HomeController> _logger;
        private readonly Mock<HttpContext> _mockHttpContext;

        private readonly Mock<IMasterDataOperations> _MasterDataOperationsMock;
        private readonly HomeController _homeController;


        public HomeControllerTests()
        {
            // Create an instance of Mock IOptions
            _optionsMock = new Mock<IOptions<ApplicationSettings>>();
            // Set IOptions<> Values property to return ApplicationSettings object
            _optionsMock.Setup(ap => ap.Value).Returns(new ApplicationSettings() { ApplicationTitle = "ASC" });
            
            _mockHttpContext = new Mock<HttpContext>();
            // Set FakeSession to HttpContext Session.
            _mockHttpContext.Setup(p => p.Session).Returns(new FakeSession());

            var loggerMock = new Mock<ILogger<HomeController>>();
            _logger = loggerMock.Object;
            // this short equivalent 
            //_logger = Mock.Of<ILogger<HomeController>>();

            _MasterDataOperationsMock = new Mock<IMasterDataOperations>();

            _homeController = new HomeController(_logger, _optionsMock.Object, _MasterDataOperationsMock.Object);

        }

        [Fact]
        public void HomeController_Index_View_Test()
        {
            // arrange
            //var homeController = new HomeController(_logger, _optionsMock.Object, _MasterDataOperationsMock.Object);
            _homeController.ControllerContext.HttpContext = _mockHttpContext.Object;
            // act
            // assert
            Assert.IsType<ViewResult>(_homeController.Index());

        }

        [Fact]
        public void HomeController_Index_NoModel_Test()
        {
            // arrange
            //var homeController = new HomeController(_logger, _optionsMock.Object);
            //act
            _homeController.ControllerContext.HttpContext = _mockHttpContext.Object;
            // Assert Model for Null
            Assert.Null((_homeController.Index() as ViewResult).ViewData.Model);
        }

        [Fact]
        public void HomeController_Index_Validation_Test()
        {
            // arrange
            //var homeController = new HomeController(_logger, _optionsMock.Object);
            //act
            _homeController.ControllerContext.HttpContext = _mockHttpContext.Object;
            //Assert
            Assert.Equal(0, (_homeController.Index() as ViewResult).ViewData.ModelState.ErrorCount);
        }

        [Fact]
        public void HomeController_Index_Session_Test()
        {
            // arrange
            //var homeController = new HomeController(_logger, _optionsMock.Object);
            _homeController.ControllerContext.HttpContext = _mockHttpContext.Object;

            //act
            _homeController.Index();
            //Assert
            // Session value with key "Test" should not be null.
            Assert.NotNull(_homeController.HttpContext.Session.GetSession<ApplicationSettings>("Test"));
        }
    }
}
