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
    public class ActivityTestFixtures:BaseTestClass
    {
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            DataBinder = new ActivityCriteriaDataBinder();
            TestHelper.TripProductType = TripProductType.Activity;
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
    }
}
