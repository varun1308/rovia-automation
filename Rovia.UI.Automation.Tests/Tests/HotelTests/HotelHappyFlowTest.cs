using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Tests.AirTests;
using Rovia.UI.Automation.Tests.Utility;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Validator;

namespace Rovia.UI.Automation.Tests.Tests.HotelTests
{
    [TestClass]
    public class HotelHappyFlowTest
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
        public void TestInitialize(){ }

        [TestCleanup]
        public void TestCleanup()
        {
            TestHelper.CleanUp();
            _logManager.SubmitLog();           
        }
           
        [TestMethod]
        [TestCategory("Sanity")]
        [DataSource("GeneralScenarios")]
        public void GeneralHotelBookFlow()
        {
            try
            {
                _logManager.StartNewLog(TestContext.DataRow["Description"].ToString());
                var criteria = DataBinder.GetCriteria(TestContext.DataRow);
                TestHelper.SetCriteria(criteria);
                TestExecutionPipeLine.Execute(criteria.Pipeline);
                _logManager.LogInformation("Passed!!!!!!!!!");
            }
            catch (Exception exception)
            {
                _logManager.LogInformation("Failed!!!!!!!!");
                Assert.Fail(exception.Message);
            }
        }
    }
}
