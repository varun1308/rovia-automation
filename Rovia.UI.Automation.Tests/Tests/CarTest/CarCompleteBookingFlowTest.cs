using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Utility;
using Tavisca.Testing.Platform;

namespace Rovia.UI.Automation.Tests.Tests.CarTest
{
    [TestClass]
    public class CarCompleteBookingFlowTest
    {
        public TestContext TestContext { get; set; }
        private static IScenarioDataBinder _dataBinder;
        private static RoviaApp _app;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _dataBinder=new CarScenarioDataBinder();
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
        public void BasicCarBooking()
        {
            _app.HomePage.DoCarSearch("LAS", "LAX", DateTime.Today.AddDays(20), DateTime.Today.AddDays(25));

            while (_app.CarResultsPage.IsWaitingVisible())
            {
                Thread.Sleep(2000);
                if (_app.CarResultsPage.IsResultsVisible())
                    break;
            }
            Assert.IsTrue(_app.CarResultsPage.IsResultsVisible(), "Results not found");
        }


        [TestMethod]
        [TestCategory("Sanity")]
        //[DataSource("SomeDataSource")]
        public void AddToCart()
        {
            _app.HomePage.Search(_dataBinder.GetCriteria(TestContext.DataRow) as CarSearchCriteria);
            //Results results =_app.CarResultsPage.ParseResults();
            //var singleResult = results.First(SomeLogic);
            //_app.CarResultsPage.AddToCart(singleResult);

        }
    }
}
