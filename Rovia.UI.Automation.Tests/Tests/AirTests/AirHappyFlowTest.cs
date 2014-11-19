using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Tests.AirTests
{
    [TestClass]
    public class AirHappyFlowTest
    {
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            TestHelper.DataBinder=new AirScenarioDataBinder();
            TestHelper.App.State.CurrentProduct="AIR";
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
                TestHelper.SetCriteria(TestContext.DataRow);
                TestHelper.Login();
                TestHelper.Search();
                TestHelper.AddToCart();
                TestHelper.CheckoutTrip();
                TestHelper.EnterPassengerDetails();
                TestHelper.EditPassengerInfoAndContinue();
                TestHelper.ConfirmPassengerDetails();
                TestHelper.PayNow();
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
