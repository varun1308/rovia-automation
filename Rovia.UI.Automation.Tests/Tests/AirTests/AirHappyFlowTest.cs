﻿using System;
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
        public void TestInitialize() { }

        [TestCleanup]
        public void TestCleanup()
        {
            TestHelper.SaveScreenShot(TestContext);
            TestHelper.CleanUp();
            _logManager.SubmitLog();
        }

        [TestMethod]
        [DataSource("Air_Amadeus_OneWay")]
        public void Air_Amadeus_OneWay()
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

        [TestMethod]
        [DataSource("Air_Amadeus_RoundTrip")]
        public void Air_Amadeus_RoundTrip()
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

        [TestMethod]
        [DataSource("Air_Amadeus_Multicity")]
        public void Air_Amadeus_Multicity()
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

        [TestMethod]
        [DataSource("Air_Mystifly_OneWay")]
        public void Air_Mystifly_OneWay()
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

        [TestMethod]
        [DataSource("Air_Mystifly_RoundTrip")]
        public void Air_Mystifly_RoundTrip()
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

        [TestMethod]
        [DataSource("Air_Mystifly_Multicity")]
        public void Air_Mystifly_Multicity()
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

        [TestMethod]
        [DataSource("Air_Sabre_OneWay")]
        public void Air_Sabre_OneWay()
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

        [TestMethod]
        [DataSource("Air_Sabre_RoundTrip")]
        public void Air_Sabre_RoundTrip()
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

        [TestMethod]
        [DataSource("Air_WorldSpan_OneWay")]
        public void Air_WorldSpan_OneWay()
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

        [TestMethod]
        [DataSource("Air_WorldSpan_RoundTrip")]
        public void Air_WorldSpan_RoundTrip()
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
