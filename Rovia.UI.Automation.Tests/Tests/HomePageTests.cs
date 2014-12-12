using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.DataBinder;
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
        public void TestInitialize(){}

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
    }
}
