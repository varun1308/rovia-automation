using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.Exceptions;
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

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            DataBinder = new AirCriteriaDataBinder();
            TestHelper.TripProductType = TripProductType.Air;
            TestHelper.Validator=new AirValidator();
        }
       
        [TestInitialize]
        public void TestInitialize(){}

        [TestCleanup]
        public void TestCleanup()
        {
            TestHelper.CleanUp();
            TestHelper.GoToHomePage();
        }

        [TestMethod]
        [TestCategory("Sanity")]
        [DataSource("AirLoginCreditCard")]
        public void RegisteredUser_BookingFlow_CreditCard()
        {
            try
            {
                var criteria = DataBinder.GetCriteria(TestContext.DataRow);
                TestHelper.SetCriteria(criteria);
                TestExecutionPipeLine.Execute(criteria.Pipeline);
            }
            catch (Exception exception)
            {
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
                var criteria   = DataBinder.GetCriteria(TestContext.DataRow);
                TestHelper.SetCriteria(criteria);
                TestExecutionPipeLine.Execute(criteria.Pipeline);
            }
            catch (Exception exception)
            {
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
                TestHelper.SetCriteria(criteria);
                TestExecutionPipeLine.Execute(criteria.Pipeline);
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
                var criteria = DataBinder.GetCriteria(TestContext.DataRow);
                TestHelper.SetCriteria(criteria);
                TestExecutionPipeLine.Execute(criteria.Pipeline);
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }
        }

        [TestMethod]
        [DataSource("AirGuestCreditCard")]
        public void SetAirFiltersAndMatrix()
        {
            try
            {
                TestHelper.SetCriteria(new AirCriteriaDataBinder().GetCriteria(TestContext.DataRow));
                TestHelper.Search();
                Assert.IsTrue(TestHelper.SetFilters(),"Set Filter Matrix failed");
            }
            catch (BaseException exception)
            {
                Assert.Fail(exception.Message);
            }
        }
    }
}
