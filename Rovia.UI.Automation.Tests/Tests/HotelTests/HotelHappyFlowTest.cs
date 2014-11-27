using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Tests.AirTests;
using Rovia.UI.Automation.Tests.Utility;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Tests.HotelTests
{
    class HotelHappyFlowTest
    {
        public TestContext TestContext { get; set; }
        public static ICriteriaDataBinder DataBinder { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            DataBinder = new HotelCriteriaDatabinder();
            TestHelper.TripProductType=TripProductType.Hotel;
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
            TestHelper.GoToHomePage();
           
        }


        [TestMethod]
        [TestCategory("Sanity")]
        [DataSource("AirGeneralDataSource")]
        public void AirSearchTest()
        {
            try
            {
                TestHelper.SetCriteria(DataBinder.GetCriteria(TestContext.DataRow));
                TestHelper.Login();
                TestHelper.Search();
                //TestHelper.AddToCart();
                //TestHelper.CheckoutTrip();
                //TestHelper.EnterPassengerDetails();
                //TestHelper.EditPassengerInfoAndContinue();
                //TestHelper.ConfirmPassengerDetails();
                //TestHelper.PayNow();
                //_app.HomePage.Search(criteria);
                //_app.AirResultsPage.AddToCart(TestHelper.ApplySpecialCriteria(criteria.SpecialCriteria));
            }
            catch (Exception exception)
            {
                Assert.Fail(exception.Message);
            }
        }
    }
}
