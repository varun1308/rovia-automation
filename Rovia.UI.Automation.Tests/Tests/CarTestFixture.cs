namespace Rovia.UI.Automation.Tests.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using DataBinder;
    using Logger;
    using ScenarioObjects;
    using Utility;

    // Test Class holding Car Product specific tests
    [TestClass]
    public class CarTestFixture
    {
        private static LogManager _logManager;
        public TestContext TestContext { get; set; }
        public static ICriteriaDataBinder DataBinder { get; set; }
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            DataBinder = new CarCriteriaDataBinder();
            TestHelper.TripProductType = TripProductType.Car;
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

        //Test executing car pick up as Airport and drop off as Airport booking flow for all scenarios
        [TestMethod, DataSource("Car_Airport_To_Airport"), TestCategory("Sanity")]
        public void Car_Airport_To_Airport()
        {
            Execute();
        }

        //Test executing car pick up as Airport and drop off as City booking flow for all scenarios
        [TestMethod, DataSource("Car_Airport_To_City"), TestCategory("Sanity")]
        public void Car_Airport_To_City()
        {
            Execute();
        }

        //Test executing car pick up as Airport and same drop off location booking flow for all scenarios
        [TestMethod, DataSource("Car_Airport_To_SameAsPickUp"), TestCategory("Sanity")]
        public void Car_Airport_To_SameAsPickUp()
        {
            Execute();
        }

        //Test executing car pick up as City and drop off as Airport booking flow for all scenarios
        [TestMethod, DataSource("Car_City_To_Airport"), TestCategory("Sanity")]
        public void Car_City_To_Airport()
        {
            Execute();
        }

        //Test executing car pick up as City and drop off as City booking flow for all scenarios
        [TestMethod, DataSource("Car_City_To_City"), TestCategory("Sanity")]
        public void Car_City_To_City()
        {
            Execute();
        }

        //Test executing car pick up as City and same drop off loaction booking flow for all scenarios
        [TestMethod, DataSource("Car_City_To_SameAsPickUp"), TestCategory("Sanity")]
        public void Car_City_To_SameAsPickUp()
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
