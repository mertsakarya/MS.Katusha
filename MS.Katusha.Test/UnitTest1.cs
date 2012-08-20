using System.Data.Entity;
using System.IO;
using System.Web;
using System.Web.Mvc;
using Http.TestLibrary;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Services.Configuration;
using MS.Katusha.Services.Configuration.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autofac;
using MS.Katusha.DependencyManagement;
using Autofac.Integration.Mvc;
using RazorEngine;

namespace MS.Katusha.Test
{
    [TestClass]
    public class UnitTest1
    {
        //private AutoMock _autoMock;
        private HttpSimulator _httpSimulator;
        
        private TestContext testContextInstance;
        private string _mailTemplatesFolder;
        private SettingsData _settings;
        private INotificationService _notificationService;
        private IUserService _userService;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestInitialize]
        public void Setup()
        {
            Database.SetInitializer(new KatushaContextInitializer());

            var builder = new ContainerBuilder();
            //builder.RegisterControllers(typeof(MvcApplication).Assembly);
            DependencyRegistrar.Build(builder);
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            DependencyRegistrar.Build(builder);

            _httpSimulator = new HttpSimulator().SimulateRequest();

            _settings = KatushaConfigurationManager.Instance.GetSettings();

            _notificationService = DependencyResolver.Current.GetService<INotificationService>();
            _userService = DependencyResolver.Current.GetService<IUserService>();

        }

        [TestCleanup]
        public void Teardown()
        {
            _httpSimulator.Dispose();
        }

        [TestMethod]
        public void ProductMonthlyKatushaExists()
        {
            var productService = DependencyResolver.Current.GetService<IProductService>();
            var product = productService.GetProductByName(ProductNames.MonthlyKatusha);
            Assert.IsNotNull(product);
        }

        [TestMethod]
        public void UserMertikoExists()
        {
            var model = _userService.GetUser("mertiko");
            Assert.AreEqual(model.UserName, "mertiko");
        }


        [TestMethod]
        public void SiteDeployedMailSend()
        {
            var model = _userService.GetUser("mertiko");
            var result = _notificationService.SiteDeployed(model);
        }

        [TestMethod]
        public void SiteDeployedCanBeRendered()
        {
            const string templateName = "SiteDeployed_en.cshtml";
            Razor.Compile<User>("@model MS.Katusha.Domain.Entities.User\r\n<h1>@Model.UserName</h1>", templateName);
            var model = _userService.GetUser("mertiko");
            var result = Razor.Run(templateName, model);
            Assert.AreEqual(result, "<h1>mertiko</h1>");
        }

    }
}
