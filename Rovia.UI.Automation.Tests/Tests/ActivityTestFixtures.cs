namespace Rovia.UI.Automation.Tests.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using DataBinder;
    using Logger;
    using ScenarioObjects;
    using Utility;

    // Test Class holding Activity Product specific tests
    [TestClass]
    public class ActivityTestFixtures
    {
        private static LogManager _logManager;
        public TestContext TestContext { get; set; }
        public static ICriteriaDataBinder DataBinder { get; set; }
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            DataBinder = new ActivityCriteriaDataBinder();
            TestHelper.TripProductType = TripProductType.Activity;
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
        
        //Test executing Activity product search to book flow
        [TestMethod, DataSource("ActivityCompleteBookingFlows"), TestCategory("Sanity")]
        public void ActivityCompleteBookingFlowTests()
        {
            Execute();
        }

        //Test executing Activity results page filters and matrix validations
        [TestMethod, DataSource("ActivityMatrixSortAndFilters"), TestCategory("Sanity")]
        public void ActivityMatrixSortAndFiltersTests()
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
