using System;
using System.Collections.Generic;
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
            //Assert.IsTrue(_app.HomePage.IsVisible(), "Could not Load Home Page!");
            //Thread.Sleep(500);
        }

        [TestCleanup]
        public void TestCleanup()
        {
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
        [TestCategory("Sanity")]
        [DataSource("AirGeneralDataSource")]
        public void PreferedCust_BookingFlow_CreditCard_Success()
        {
            TestHelper.SetCriteria(new AirCriteriaDataBinder().GetCriteria(TestContext.DataRow));
            TestHelper.Login();
            TestHelper.Search();
            TestHelper.AddToCart();
            TestHelper.CheckoutTrip();
            TestHelper.EnterPassengerDetails();
            TestHelper.ConfirmPassengerDetails();
            TestHelper.PayNow();
            Thread.Sleep(1000);

            //check if itinerary available or price not changed before actual payment




        }

        [TestMethod]
        [TestCategory("Sanity")]
        [DataSource("AirGeneralDataSource")]
        public void GuestUser_BookingFlow_CreditCard_Success()
        {
            TestHelper.SetCriteria(new AirCriteriaDataBinder().GetCriteria(TestContext.DataRow));
            TestHelper.Search();
            TestHelper.AddToCart();
            TestHelper.CheckoutTrip();
            
            TestHelper.PayNow();

        }

        [TestMethod]
        [TestCategory("Sanity")]
        [DataSource("AirGeneralDataSource")]
        public void PreferedCust_BookingFlow_RoviaBucks_Success()
        {
            TestHelper.SetCriteria(new AirCriteriaDataBinder().GetCriteria(TestContext.DataRow));
            TestHelper.Login();
            TestHelper.Search();
            TestHelper.AddToCart();
            TestHelper.CheckoutTrip();
            TestHelper.EnterPassengerDetails();
            TestHelper.ConfirmPassengerDetails();
            TestHelper.PayNow();
        }

        [TestMethod]
        [DataSource("AirGeneralDataSource")]
        public void SetAirFilters_AirResultPage()
        {
            try
            {
                TestHelper.SetCriteria(new AirCriteriaDataBinder().GetCriteria(TestContext.DataRow));
                TestHelper.Search();
                //as of now values are static need to take from data sheet
                TestHelper.SetAirFilters();
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }
        }

        [TestMethod]
        [DataSource("AirGeneralDataSource")]
        public void SetAirMatrix_AirResultPage()
        {
            try
            {
                TestHelper.SetCriteria(new AirCriteriaDataBinder().GetCriteria(TestContext.DataRow));
                TestHelper.Search();
                TestHelper.SetMatrixAirline();
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }
        }
    }
}
