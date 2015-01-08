using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Tests
{
    [TestClass]
    public class ActivityTestFixtures
    {
        public TestContext TestContext { get; set; }
        public static ICriteriaDataBinder DataBinder { get; set; }
        private static LogManager _logManager;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            //DataBinder = new ActivityCriteriaDataBinder();
            TestHelper.TripProductType = TripProductType.Activity;
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



        [TestMethod, TestCategory("Sanity")]
        //public void ActivityDummyTest()
        //{
        //    Execute();
        //}

        private void Execute()
        {
            try
            {
                //var criteria = DataBinder.GetCriteria(TestContext.DataRow);
                var criteria = new ActivitySearchCriteria()
                    {
                        Description = "Activity-DummyTest",
                        ShortLocation = "Singapore",
                        Location = "Singapore, SG",
                        FromDate = DateTime.Parse("02/06/2015"),
                        ToDate = DateTime.Parse("02/13/2015"),
                        Pipeline = "Search|AddTOCart",
                        Passengers = new Passengers()
                            {
                                Adults = 2,
                                Children = 1,
                                Infants = 1
                            },
                        Filters = new Filters()
                            {
                                PostSearchFilters = new ActivityPostSearchFilters()
                                    {
                                        //ActivityName = "Experience Trick Art",
                                        //PriceRange = new PriceRange()
                                        //    {
                                        //        Max = 10,
                                        //        Min=10
                                        //    },
                                        Matrix = new ActivityMatrix() {Category = "Water Sports"}
                                    }
                            }
                    };
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
