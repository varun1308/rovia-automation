namespace Rovia.UI.Automation.Tests.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using DataBinder;
    using Logger;
    using ScenarioObjects;
    using Utility;

    // Test Class holding Car Product specific tests
    [TestClass]
    public class CarTestFixture:BaseTestClass
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            DataBinder = new CarCriteriaDataBinder();
            TestHelper.TripProductType = TripProductType.Car;
        }

        //Test executing car pick up as Airport and drop off as Airport booking flow for all scenarios
        [TestMethod, DataSource("Car_Airport_To_Airport"), TestCategory("Sanity")]
        public void Car_Airport_To_Airport()
        {
            Execute();
        }

        //Test executing car pick up as Airport and drop off as City booking flow for all scenarios
        [TestMethod, DataSource("Car_Airport_To_City"), TestCategory("Sanity")]
        public void Car_Airport_To_City()
        {
            Execute();
        }

        //Test executing car pick up as Airport and same drop off location booking flow for all scenarios
        [TestMethod, DataSource("Car_Airport_To_SameAsPickUp"), TestCategory("Sanity")]
        public void Car_Airport_To_SameAsPickUp()
        {
            Execute();
        }

        //Test executing car pick up as City and drop off as Airport booking flow for all scenarios
        [TestMethod, DataSource("Car_City_To_Airport"), TestCategory("Sanity")]
        public void Car_City_To_Airport()
        {
            Execute();
        }

        //Test executing car pick up as City and drop off as City booking flow for all scenarios
        [TestMethod, DataSource("Car_City_To_City"), TestCategory("Sanity")]
        public void Car_City_To_City()
        {
            Execute();
        }

        //Test executing car pick up as City and same drop off loaction booking flow for all scenarios
        [TestMethod, DataSource("Car_City_To_SameAsPickUp"), TestCategory("Sanity")]
        public void Car_City_To_SameAsPickUp()
        {
            Execute();
        }
    }
}
