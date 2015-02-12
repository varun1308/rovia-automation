using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.DataBinder;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.Tests.Utility;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Tests
{
    // Test Class holding Hotel Product specific tests
    [TestClass]
    public class HotelTestFixture:BaseTestClass
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            DataBinder = new HotelCriteriaDatabinder();
            TestHelper.TripProductType = TripProductType.Hotel;
        }

        //Test executing Hotel AmadeusWS supplier booking flow
        [TestMethod,TestCategory("Sanity"),DataSource("AmedusWSHotelBookingFlow")]
        public void Hotel_AmedusWsBookingFlow()
        {
            Execute();
        }

        //Test executing Hotel HotelsCom supplier booking flow
        [TestMethod, TestCategory("Sanity"), DataSource("HotelsComHotelBookingFlow")]
        public void Hotel_HotelsComBookingFlow()
        {
            Execute();
        }

        //Test executing Hotel HotelBeds supplier booking flow
        [TestMethod, TestCategory("Sanity"), DataSource("HotelBedsHotelBookingFlow")]
        public void HotelBedsHotelBookingFlow()
        {
            Execute();
        }

        //Test executing Hotel Pegasus supplier booking flow
        [TestMethod, TestCategory("Sanity"), DataSource("PegasusHotelBookingFlow")]
        public void Hotel_PegasusBookingFlow()
        {
            Execute();
        }

        //Test executing Hotel PegasusSpl supplier booking flow
        [TestMethod, TestCategory("Sanity"), DataSource("PegasussplHotelBookingFlow")]
        public void Hotel_PegasussplBookingFlow()
        {
            Execute();
        }

        //Test executing Hotel PricelineV3 supplier booking flow
        [TestMethod, TestCategory("Sanity"), DataSource("PriceLineV3HotelBookingFlow")]
        public void Hotel_PriceLineV3BookingFlow()
        {
            Execute();
        }

        //Test executing Hotel TouricoTGS supplier booking flow
        [TestMethod, TestCategory("Sanity"), DataSource("TouricoTGSHotelBookingFlow")]
        public void Hotel_TouricoTGSBookingFlow()
        {
            Execute();
        }

        //Test executing Hotel results page validations for filters and matrix
        [TestMethod, TestCategory("Sanity"), DataSource("FiltersAndMatrixScenarios")]
        public void Hotel_FiltersAndMatrixTests()
        {
            Execute();
        }
    }
}
