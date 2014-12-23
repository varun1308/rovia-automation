using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Tests
{
    [TestClass]
    public class CarTestFixture
    {
        public TestContext TestContext { get; set; }
        public static ICriteriaDataBinder DataBinder { get; set; }
        private static LogManager _logManager;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            DataBinder = new CarCriteriaDataBinder();
            TestHelper.TripProductType = TripProductType.Car;
            _logManager = LogManager.GetInstance();
        }

        [TestInitialize]
        public void TestInitialize() { }

        [TestCleanup]
        public void TestCleanup()
        {
            TestHelper.SaveScreenShot(TestContext);
            _logManager.SubmitLog(TestHelper.SessionId);
            TestHelper.CleanUp();
        }


        [TestMethod, DataSource("Car_Airport_To_Airport"), TestCategory("Sanity")]
        public void Car_Airport_To_Airport()
        {
            Execute();
        }

        [TestMethod, DataSource("Car_Airport_To_City"), TestCategory("Sanity")]
        public void Car_Airport_To_City()
        {
            Execute();
        }

        [TestMethod, DataSource("Car_Airport_To_SameAsPickUp"), TestCategory("Sanity")]
        public void Car_Airport_To_SameAsPickUp()
        {
            Execute();
        }

        [TestMethod, DataSource("Car_City_To_Airport"), TestCategory("Sanity")]
        public void Car_City_To_Airport()
        {
            Execute();
        }

        [TestMethod, DataSource("Car_City_To_City"), TestCategory("Sanity")]
        public void Car_City_To_City()
        {
            Execute();
        }

        [TestMethod, DataSource("Car_City_To_SameAsPickUp"), TestCategory("Sanity")]
        public void Car_City_To_SameAsPickUp()
        {
            Execute();
        }

        public void Execute()
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
