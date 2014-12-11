using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Utility;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Validator;

namespace Rovia.UI.Automation.Tests.Tests.AirTests
{
    [TestClass]
    public class AirHappyFlowTest
    {
        public TestContext TestContext { get; set; }
        public static ICriteriaDataBinder DataBinder { get; set; }
        private static LogManager _logManager;
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            DataBinder = new AirCriteriaDataBinder();
            TestHelper.TripProductType = TripProductType.Air;
            TestHelper.Validator = new AirValidator();
            _logManager = LogManager.GetInstance();
        }
       
        [TestInitialize]
        public void TestInitialize(){}

        [TestCleanup]
        public void TestCleanup()
        {
            TestHelper.CleanUp();
            _logManager.SubmitLog();
        }

        [TestMethod]
        [TestCategory("Sanity")]
        [DataSource("AirLoginCreditCard")]
        public void RegisteredUser_BookingFlow_CreditCard()
        {
            try
            {
                var criteria = DataBinder.GetCriteria(TestContext.DataRow);
                _logManager.StartNewLog(criteria.Description);
                TestHelper.SetCriteria(criteria);
                TestExecutionPipeLine.Execute(criteria.Pipeline);
                _logManager.LogInformation("Passed!!!!!!!!!");
                TestHelper.TakeScreenShot(TestContext);
            }
            catch (Exception exception)
            {
                 TestHelper.TakeScreenShot(TestContext);
                _logManager.LogInformation("Failed!!!!!!!!");
                Assert.Fail(exception.Message);
            }
        }

        [TestMethod]
        [TestCategory("Sanity")]
        [DataSource("AirGuestCreditCard")]
        public void GuestUser_BookingFlow_CreditCard()
        {
            try
            {
                var criteria = DataBinder.GetCriteria(TestContext.DataRow);
                _logManager.StartNewLog(criteria.Description);
                TestHelper.SetCriteria(criteria);
                TestExecutionPipeLine.Execute(criteria.Pipeline);
                _logManager.LogInformation("Passed!!!!!!!!!");
                TestHelper.TakeScreenShot(TestContext);
            }
            catch (Exception exception)
            {
                TestHelper.TakeScreenShot(TestContext);
                _logManager.LogInformation("Failed!!!!!!!!");
                Assert.Fail(exception.Message);
            }
        }

        [TestMethod]
        [TestCategory("Sanity")]
        [DataSource("AirLoginRoviaBucks")]
        public void RegisteredUser_BookingFlow_RoviaBucks()
        {
            try
            {
                var criteria = DataBinder.GetCriteria(TestContext.DataRow);
                _logManager.StartNewLog(criteria.Description);
                TestHelper.SetCriteria(criteria);
                TestExecutionPipeLine.Execute(criteria.Pipeline);
                _logManager.LogInformation("Passed!!!!!!!!!");
                TestHelper.TakeScreenShot(TestContext);
            }
            catch (Exception exception)
            {
                 TestHelper.TakeScreenShot(TestContext);
                _logManager.LogInformation("Failed!!!!!!!!");
                Assert.Fail(exception.Message);
            }
        }
    }
}
