using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Utility;
using Rovia.UI.Automation.ScenarioObjects;

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
                TestHelper.SetCriteria(DataBinder.GetCriteria(TestContext.DataRow));
                TestHelper.Login();
                TestHelper.Search();
                TestHelper.AddToCart();
                TestHelper.CheckoutTrip();
                TestHelper.EnterPassengerDetails();
                TestHelper.EditPassengerInfoAndContinue();
                TestHelper.ConfirmPassengerDetails();
                TestHelper.PayNow();
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
                TestHelper.SetCriteria(new AirCriteriaDataBinder().GetCriteria(TestContext.DataRow));
                TestHelper.Search();
                TestHelper.AddToCart();
                TestHelper.CheckoutTrip();
                TestHelper.Login();
                TestHelper.EnterPassengerDetails();
                TestHelper.ConfirmPassengerDetails();
                TestHelper.PayNow();
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
                TestHelper.SetCriteria(DataBinder.GetCriteria(TestContext.DataRow));
                TestHelper.Login();
                TestHelper.Search();
                TestHelper.AddToCart();
                TestHelper.CheckoutTrip();
                TestHelper.EnterPassengerDetails();
                TestHelper.EditPassengerInfoAndContinue();
                TestHelper.ConfirmPassengerDetails();
                TestHelper.PayNow();
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
