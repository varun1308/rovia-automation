using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.Tests.Utility;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Validator;

namespace Rovia.UI.Automation.Tests.Tests
{
    [TestClass]
    public class HotelTextFixture
    {
        public TestContext TestContext { get; set; }
        public static ICriteriaDataBinder DataBinder { get; set; }
        private static LogManager _logManager;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            DataBinder = new HotelCriteriaDatabinder();
            TestHelper.TripProductType = TripProductType.Hotel;
            TestHelper.Validator = new AirValidator();
            _logManager = LogManager.GetInstance();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            TestHelper.InitializeTest();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TestHelper.SaveScreenShot(TestContext);
            _logManager.SubmitLog(TestHelper.TripsErrorUI);
        }

        [TestMethod,TestCategory("Sanity"),DataSource("AmedusWSHotelBookingFlow")]
        public void Hotel_AmedusWsBookingFlow()
        {
            Execute();
        }

        [TestMethod, TestCategory("Sanity"), DataSource("HotelsComHotelBookingFlow")]
        public void Hotel_HotelsComBookingFlow()
        {
            Execute();
        }

        [TestMethod, TestCategory("Sanity"), DataSource("HotelBedsHotelBookingFlow")]
        public void HotelBedsHotelBookingFlow()
        {
            Execute();
        }

        [TestMethod, TestCategory("Sanity"), DataSource("PegasusHotelBookingFlow")]
        public void Hotel_PegasusBookingFlow()
        {
            Execute();
        }

        [TestMethod, TestCategory("Sanity"), DataSource("PegasussplHotelBookingFlow")]
        public void Hotel_PegasussplBookingFlow()
        {
            Execute();
        }

        [TestMethod, TestCategory("Sanity"), DataSource("PriceLineV3HotelBookingFlow")]
        public void Hotel_PriceLineV3BookingFlow()
        {
            Execute();
        }

        [TestMethod, TestCategory("Sanity"), DataSource("TuricoTGSHotelBookingFlow")]
        public void Hotel_TuricoTgsBookingFlow()
        {
            Execute();
        }

        [TestMethod, TestCategory("Sanity"), DataSource("FiltersAndMatrixScenarios")]
        public void Hotel_FiltersAndMatrixTests()
        {
            Execute();
        }

        private void Execute()
        {
            try
            {
                var criteria = DataBinder.GetCriteria(TestContext.DataRow);
                _logManager.StartNewLog(criteria.Description);
                TestHelper.SetCriteria(criteria);
                TestExecutionPipeLine.Execute(criteria.Pipeline);
                _logManager.LogInformation("Test Passed.");
            }
            catch (Exception exception)
            {
                _logManager.LogInformation("Test Failed.");
                Assert.Fail(exception.Message);
            }
        }
    }
}
