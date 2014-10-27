using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Tests
{
    [TestClass]
    public class HomePageTests
    {
        private static RoviaApp _app;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
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
        }

        [TestMethod]
        [TestCategory("Sanity")]
        public void ShouldShowHomePage()
        {
            Assert.IsTrue(_app.HomePage.IsVisible(), "Home was unavailable");

        }

        [TestMethod]
        [TestCategory("Sanity")]
        public void CheckFlightsWorking()
        {
            _app.HomePage.DoAirSearch("DFW", "MIA", DateTime.Today.AddDays(7),
                false, DateTime.Today.AddDays(14), 1, 0, 0);

            while (_app.AirResultsPage.IsWaitingVisible())
            {
                Thread.Sleep(2000);
                if (_app.AirResultsPage.IsResultsVisible())
                    break;
            }
            Assert.IsTrue(_app.AirResultsPage.IsResultsVisible(), "Results not found");
        }

    }
}
