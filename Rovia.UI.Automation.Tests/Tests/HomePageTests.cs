using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Model;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Tests
{
    [TestClass]
    public class HomePageTests
    {
        private static RoviaApp _app;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            _app = TestHelper.App;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            Assert.IsTrue(_app.HomePage.IsVisible(), "Could not Load Home Page!");
            Thread.Sleep(500);
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        [TestCategory("Sanity")]
        public void ShouldShowHomePage()
        {
            Assert.IsTrue(_app.HomePage.IsVisible(), "Home was unavailable");
        }

        [TestMethod]
        [TestCategory("Sanity")]
        public void CheckFlightsWorking()
        {
            AirSearchScenario airScenario = new AirSearchScenario()
            {
                Adults = 1,
                Childs = 0,
                Infants = 0,
                SearchType = SearchType.Return,
                AirportPairs = new List<AirportPair>{
                    new AirportPair()
                {
                    FromLocation = "LAS",
                    ToLocation = "LAX",
                    DepartureDateTime = DateTime.Today.AddDays(7)
                },
                   new AirportPair()
                {
                    FromLocation = "LAX",
                    ToLocation = "MIA",
                    DepartureDateTime = DateTime.Today.AddDays(23)
                }
                }
            };
            _app.HomePage.DoAirSearch(airScenario);
            
            while (_app.AirResultsPage.IsWaitingVisible())
            {
                Thread.Sleep(2000);
                if (_app.AirResultsPage.IsResultsVisible())
                    break;
            }
            Assert.IsTrue(_app.AirResultsPage.IsResultsVisible(), "Results not found");
        }

        [TestMethod]
        [TestCategory("Sanity")]
        public void AddFlightToCart()
        {
            AirSearchScenario airScenario = new AirSearchScenario()
            {
                Adults = 1,
                Childs = 0,
                Infants = 0,
                SearchType = SearchType.OneWay,
                AirportPairs = new List<AirportPair>{new AirportPair()
                {
                    FromLocation = "LAS",
                    ToLocation = "LAX",
                    DepartureDateTime = DateTime.Today.AddDays(7)
                }}
            };
            _app.HomePage.DoAirSearch(airScenario);

            while (_app.AirResultsPage.IsWaitingVisible())
            {
                Thread.Sleep(2000);
                if (_app.AirResultsPage.IsResultsVisible())
                    Assert.IsTrue(_app.AirResultsPage.AddToCart(), "Itinerary not available");
            }
        }
    }
}
