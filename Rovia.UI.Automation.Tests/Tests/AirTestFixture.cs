namespace Rovia.UI.Automation.Tests.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using DataBinder;
    using Logger;
    using Utility;
    using ScenarioObjects;

    // Test Class holding Air Product specific tests
    [TestClass]
    public class AirTestFixture
    {
        private static LogManager _logManager;
        public TestContext TestContext { get; set; }
        private static ICriteriaDataBinder DataBinder { get; set; }
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            DataBinder = new AirCriteriaDataBinder();
            TestHelper.TripProductType = TripProductType.Air;
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

        //Test executing Air Amadeus Supplier for OneWay Trip
        [TestMethod, DataSource("Air_Amadeus_OneWay"), TestCategory("Sanity")]
        public void Air_Amadeus_OneWay()
        {
            Execute();
        }

        //Test executing Air Amadeus Supplier for RoundTrip Trip
        [TestMethod, DataSource("Air_Amadeus_RoundTrip"), TestCategory("Sanity")]
        public void Air_Amadeus_RoundTrip()
        {
            Execute();
        }

        //Test executing Air Amadeus Supplier for Multicity Trip
        [TestMethod, DataSource("Air_Amadeus_Multicity"), TestCategory("Sanity")]
        public void Air_Amadeus_Multicity()
        {
            Execute();
        }

        //Test executing Air Mystifly Supplier for OneWay Trip
        [TestMethod, DataSource("Air_Mystifly_OneWay"), TestCategory("Sanity")]
        public void Air_Mystifly_OneWay()
        {
            Execute();
        }

        //Test executing Air Mystifly Supplier for RoundTrip Trip
        [TestMethod, DataSource("Air_Mystifly_RoundTrip"), TestCategory("Sanity")]
        public void Air_Mystifly_RoundTrip()
        {
            Execute();
        }

        //Test executing Air Mystifly Supplier for Multicity Trip
        [TestMethod, DataSource("Air_Mystifly_Multicity"), TestCategory("Sanity")]
        public void Air_Mystifly_Multicity()
        {
            Execute();
        }

        //Test executing Air Sabre Supplier for OneWay Trip
        [TestMethod, DataSource("Air_Sabre_OneWay"), TestCategory("Sanity")]
        public void Air_Sabre_OneWay()
        {
            Execute();
        }

        //Test executing Air Sabre Supplier for OneWay Trip
        [TestMethod, DataSource("Air_Sabre_RoundTrip"), TestCategory("Sanity")]
        public void Air_Sabre_RoundTrip()
        {
            Execute();
        }

        //Test executing Air WorldSpan Supplier for OneWay Trip
        [TestMethod, DataSource("Air_WorldSpan_OneWay"), TestCategory("Sanity")]
        public void Air_WorldSpan_OneWay()
        {
            Execute();
        }

        //Test executing Air WorldSpan Supplier for RoundTrip Trip
        [TestMethod, DataSource("Air_WorldSpan_RoundTrip"), TestCategory("Sanity")]
        public void Air_WorldSpan_RoundTrip()
        {
            Execute();
        }

        //Executes the tests depending on pipeline and given criteria
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
