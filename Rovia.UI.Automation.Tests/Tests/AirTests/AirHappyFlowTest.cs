using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Tests.AirTests
{
    [TestClass]
    public class AirHappyFlowTest
    {
        public TestContext TestContext { get; set; }

        private static RoviaApp _app;
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestHelper.DataBinder=new AirScenarioDataBinder();
            _app = TestHelper.App;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Assert.IsTrue(_app.HomePage.IsVisible(), "Could not Load Home Page!");
            Thread.Sleep(500);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _app.GoToHomePage();
        }


        [TestMethod]
        [TestCategory("Sanity")]
        [DataSource("AirGeneralDataSource")]
        public void AirSearchTest()
        {
            try
            {
                TestHelper.SetCriteria(TestContext.DataRow);
                TestHelper.Login();
                TestHelper.Search();
                //TestHelper.AddToCart();
                //_app.HomePage.Search(criteria);
                //_app.AirResultsPage.AddToCart(TestHelper.ApplySpecialCriteria(criteria.SpecialCriteria));
            }
            catch (Exception exception)
            {
                
                Assert.Fail(exception.Message);
            }
        }

        

    }
}
