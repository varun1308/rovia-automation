using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Utility;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Tests.AirTests
{
    [TestClass]
    public class AirHappyFlowTest
    {
        public TestContext TestContext { get; set; }
        public static ICriteriaDataBinder DataBinder { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            DataBinder=new AirCriteriaDataBinder();
            TestHelper.TripProductType=TripProductType.Air;
        }
       
        [TestInitialize]
        public void TestInitialize()
        {
            //Assert.IsTrue(_app.HomePage.IsVisible(), "Could not Load Home Page!");
            //Thread.Sleep(500);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TestHelper.GoToHomePage();
        }


       
    }
}
