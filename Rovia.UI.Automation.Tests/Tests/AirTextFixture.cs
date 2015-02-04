﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.Tests.Utility;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Tests
{
    [TestClass]
    public class AirTextFixture
    {
        public TestContext TestContext { get; set; }
        public static ICriteriaDataBinder DataBinder { get; set; }
        private static LogManager _logManager;

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

        [TestMethod,DataSource("Air_Amadeus_OneWay"),TestCategory("Sanity")]
        public void Air_Amadeus_OneWay()
        {
            Execute();
        }

        [TestMethod, DataSource("Air_Amadeus_RoundTrip"), TestCategory("Sanity")]
        public void Air_Amadeus_RoundTrip()
        {
            Execute();
        }

        [TestMethod,DataSource("Air_Amadeus_Multicity"),TestCategory("Sanity")]
        public void Air_Amadeus_Multicity()
        {
            Execute();
        }

        [TestMethod, DataSource("Air_Mystifly_OneWay"),TestCategory("Sanity")]
        public void Air_Mystifly_OneWay()
        {
            Execute();
        }

        [TestMethod, DataSource("Air_Mystifly_RoundTrip"),TestCategory("Sanity")]
        public void Air_Mystifly_RoundTrip()
        {
            Execute();
        }

        [TestMethod, DataSource("Air_Mystifly_Multicity"),TestCategory("Sanity")]
        public void Air_Mystifly_Multicity()
        {
            Execute();
        }

        [TestMethod, DataSource("Air_Sabre_OneWay"),TestCategory("Sanity")]
        public void Air_Sabre_OneWay()
        {
            Execute();
        }

        [TestMethod, DataSource("Air_Sabre_RoundTrip"),TestCategory("Sanity")]
        public void Air_Sabre_RoundTrip()
        {
            Execute();
        }

        [TestMethod,DataSource("Air_WorldSpan_OneWay"),TestCategory("Sanity")]
        public void Air_WorldSpan_OneWay()
        {
            Execute();
        }

        [TestMethod,DataSource("Air_WorldSpan_RoundTrip"),TestCategory("Sanity")]
        public void Air_WorldSpan_RoundTrip()
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
