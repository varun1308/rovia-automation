using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Model;
using Rovia.UI.Automation.Tests.Tests.AirTests;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Tests
{
    [TestClass]
    public class HomePageTests
    {
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            AirHappyFlowTest.DataBinder = new AirCriteriaDataBinder();

        }

        [TestInitialize]
        public void TestInitialize()
        {
        }

        [TestCleanup]
        public void TestCleanup()
        {
            TestHelper.CleanUp();
            TestHelper.GoToHomePage();
        }

        [TestMethod]
        [TestCategory("Sanity")]
        public void ShouldShowHomePage()
        {
            try
            {
                TestHelper.GoToHomePage();
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }
        }

        [TestMethod]
        [DataSource("AirLoginCreditCard")]
        public void AirSearch()
        {
            try
            {
                TestHelper.SetCriteria(new AirCriteriaDataBinder().GetCriteria(TestContext.DataRow));
                TestHelper.Search();
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }
        }

        [TestMethod]
        [DataSource("AirLoginCreditCard")]
        public void SetAirFilters()
        {
            try
            {
                TestHelper.SetCriteria(new AirCriteriaDataBinder().GetCriteria(TestContext.DataRow));
                TestHelper.Search();
                //as of now values are static need to take from data sheet
                TestHelper.SetFilters();
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }
        }

        [TestMethod]
        [DataSource("AirLoginCreditCard")]
        public void SetAirMatrix()
        {
            try
            {
                TestHelper.SetCriteria(new AirCriteriaDataBinder().GetCriteria(TestContext.DataRow));
                TestHelper.Search();
                TestHelper.SetMatrix();
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }
        }
    }
}
