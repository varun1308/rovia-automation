using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Utility;
using InvalidOperationException = Rovia.UI.Automation.Exceptions.InvalidOperationException;

namespace Rovia.UI.Automation.Tests.Tests
{
    [TestClass]
    public class BaseTestClass
    {
        protected static LogManager _logManager;
        public TestContext TestContext { get; set; }
        protected static ICriteriaDataBinder DataBinder { get; set; }

        static BaseTestClass()
        {
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
        /// <summary>
        /// Executes all the steps inside given pipeline
        /// </summary>
        /// <param name="pipeline">Steps separated by '|' symbol</param>
        private static void ExecutePipeline(string pipeline)
        {
            try
            {
                pipeline.Split('|').ToList().ForEach(x =>
                {
                    switch (x.ToUpper())
                    {
                        case "LOGIN":
                            TestHelper.Login();
                            break;
                        case "SEARCH":
                            TestHelper.Search();
                            break;
                        case "ADDTOCART":
                            TestHelper.AddToCart();
                            break;
                        case "CHECKOUTTRIP":
                            TestHelper.CheckoutTrip();
                            break;
                        case "EDITPAXINFO":
                            TestHelper.EditPassengerInfoAndContinue();
                            break;
                        case "ENTERPAXINFO":
                            TestHelper.EnterPassengerDetails();
                            break;
                        case "CONFIRMPAXINFO":
                            TestHelper.ConfirmPassengerDetails();
                            break;
                        case "SAVETRIP":
                            TestHelper.SaveTrip();
                            break;
                        case "TRIPSTARTOVER":
                            TestHelper.TripStartOver();
                            break;
                        case "CONTINUESHOPPING":
                            TestHelper.ContinueShopping();
                            break;
                        case "SETFILTERS":
                            TestHelper.SetValidateFilters();
                            break;
                        case "PAYNOW":
                            TestHelper.PayNow();
                            break;
                        default: throw new InvalidOperationException(string.Format("{0} in Pipeline", x));
                    }
                });
            }
            catch (Exception exception)
            {
                LogManager.GetInstance().LogError(exception);
                throw;
            }
        }
        //Executes the tests depending on pipeline and given criteria
        protected void Execute()
        {
            try
            {
                var criteria = DataBinder.GetCriteria(TestContext.DataRow);
                _logManager.StartNewLog(criteria.Description);
                TestHelper.SetCriteria(criteria);
                ExecutePipeline(criteria.Pipeline);
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
