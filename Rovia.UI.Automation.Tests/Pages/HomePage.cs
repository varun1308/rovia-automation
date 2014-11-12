using System;
using System.Collections.Generic;
using System.Linq;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Utility;
using System.Threading;

namespace Rovia.UI.Automation.Tests.Pages
{
    public class HomePage : UIPage
    {
        

        #region Common
        internal bool IsVisible()
        {
            var div = WaitAndGetBySelector("divHome", ApplicationSettings.TimeOut.Slow);
            var country = WaitAndGetBySelector("country", ApplicationSettings.TimeOut.Slow);
            Thread.Sleep(500);
            if (country != null && country.Displayed)
            {
                country.SendKeys("United States");
                WaitAndGetBySelector("saveCountry", ApplicationSettings.TimeOut.Slow).Click();
            }

            return div != null && div.Displayed;
        }

        internal bool IsUserLoggedIn()
        {
            var anchor = WaitAndGetBySelector("logOut", ApplicationSettings.TimeOut.Slow);
            return anchor != null && anchor.Displayed;
        }

        internal void DoAirSearch(string from, string to, DateTime onwardDateTime, bool isReturn, DateTime returnDateTime,
            int adults = 1, int children = 0, int infants = 0)
        {
            var flights = WaitAndGetBySelector("flights", ApplicationSettings.TimeOut.Slow);

            if (flights == null || !flights.Displayed)
            {
                Assert.Fail("Flights Nav Bar not displayed.");
            }

            flights.Click();

            Thread.Sleep(500);
            var flightPanel = WaitAndGetBySelector("flightsSearchPanel", ApplicationSettings.TimeOut.Slow);

            if (flightPanel == null || !flightPanel.Displayed)
                Assert.Fail("Flights Panel not displayed.");

            //check if return or one way
            if (!isReturn)
            {
                var btnOneway = WaitAndGetBySelector("flightJourneyTypeOneWay", ApplicationSettings.TimeOut.Fast);

                if (btnOneway == null || !btnOneway.Displayed)
                    Assert.Fail("One way journey button not displayed.");

                btnOneway.Click();
            }

            //Enter from/to airports

            var input = WaitAndGetBySelector("fromAp", ApplicationSettings.TimeOut.Slow);

            input.SendKeys(from);

            input = WaitAndGetBySelector("toAp", ApplicationSettings.TimeOut.Slow);
            input.SendKeys(to);

            //enter date

            input = WaitAndGetBySelector("onwardDate", ApplicationSettings.TimeOut.Slow);

            input.SendKeys(onwardDateTime.ToString("MM/dd/yyyy"));

            if (isReturn)
            {
                input = WaitAndGetBySelector("returnDate", ApplicationSettings.TimeOut.Slow);

                input.SendKeys(onwardDateTime.ToString("MM/dd/yyyy"));
            }

            var combo = WaitAndGetBySelector("adults", ApplicationSettings.TimeOut.Fast);

            combo.SelectFromDropDown(adults.ToString());

            combo = WaitAndGetBySelector("children", ApplicationSettings.TimeOut.Fast);

            combo.SelectFromDropDown(children.ToString());

            combo = WaitAndGetBySelector("infants", ApplicationSettings.TimeOut.Fast);

            combo.SelectFromDropDown(infants.ToString());

            var btnSearch = WaitAndGetBySelector("btnAirSearch", ApplicationSettings.TimeOut.Fast);

            btnSearch.Click();
        }



        internal void GoToLoginPage()
        {
            WaitAndGetBySelector("lnkLogIn", ApplicationSettings.TimeOut.Fast).Click();
        }

        internal void LogOut()
        {
            WaitAndGetBySelector("logOut", ApplicationSettings.TimeOut.Fast).Click();
        } 
        #endregion

        #region CarSearch
        public void DoCarSearch(string from, string to, DateTime fromDate, DateTime toDate)
        {
            Thread.Sleep(500);

            //enter date

            WaitAndGetBySelector("fromDateC", ApplicationSettings.TimeOut.Fast).SendKeys(fromDate.ToString("MM/dd/yyyy"));

            WaitAndGetBySelector("toDateC", ApplicationSettings.TimeOut.Fast).SendKeys(toDate.ToString("MM/dd/yyyy"));

            //Enter from/to

            WaitAndGetBySelector("fromC", ApplicationSettings.TimeOut.Slow).SendKeys(from);

            WaitAndGetBySelector("toC", ApplicationSettings.TimeOut.Fast).SendKeys(to);

            WaitAndGetBySelector("btnCarSearch", ApplicationSettings.TimeOut.Fast).Click();

        }

        public void Search(CarSearchCriteria scenario)
        {
            SelectRentalsPanel();

            //enter date

            WaitAndGetBySelector("fromDateC", ApplicationSettings.TimeOut.Fast).SendKeys(scenario.FromDate.ToString("MM/dd/yyyy"));

            WaitAndGetBySelector("toDateC", ApplicationSettings.TimeOut.Fast).SendKeys(scenario.ToDate.ToString("MM/dd/yyyy"));

            //Enter from/to
            // todo Handle radius and locatiotypes
            WaitAndGetBySelector("fromC", ApplicationSettings.TimeOut.Slow).SendKeys(scenario.PickUpLocation);

            WaitAndGetBySelector("toC", ApplicationSettings.TimeOut.Fast).SendKeys(scenario.DropOffLocation);

            WaitAndGetBySelector("btnCarSearch", ApplicationSettings.TimeOut.Fast).Click();
        }

        private void SelectRentalsPanel()
        {
            var rentals = WaitAndGetBySelector("rentals", ApplicationSettings.TimeOut.Slow);
            if (rentals == null || !rentals.Displayed)
                Assert.Fail("Rentals Nav Bar not displayed.");
            rentals.Click();

            var rentalPanel = WaitAndGetBySelector("rentalsSearchPanel", ApplicationSettings.TimeOut.Fast);
            if (rentalPanel == null || !rentalPanel.Displayed)
                Assert.Fail("rentals Panel not displayed.");
            Thread.Sleep(500);
        } 
        #endregion

        #region AirSearch
        private void SelectFlightPanel()
        {
            var flights = WaitAndGetBySelector("flights", ApplicationSettings.TimeOut.Slow);
            if (flights == null || !flights.Displayed)
                Assert.Fail("Flights Nav Bar not displayed.");
            flights.Click();

            Thread.Sleep(500);

            var flightPanel = WaitAndGetBySelector("flightsSearchPanel", ApplicationSettings.TimeOut.Slow);
            if (flightPanel == null || !flightPanel.Displayed)
                Assert.Fail("Flights Panel not displayed.");

        }

        private void SetFlightType(SearchType searchType)
        {
            switch (searchType)
            {
                case SearchType.OneWay: SelectOneWayFlight();
                    break;
                case SearchType.RoundTrip: SelectRoundTripFlight();
                    break;
                case SearchType.MultiCity: SelectMulticityFlight();
                    break;

            }
        } 
        private void SelectOneWayFlight()
        {
            try
            {
                WaitAndGetBySelector("flightJourneyTypeOneWay", ApplicationSettings.TimeOut.Fast).Click();
            }
            catch (Exception ex)
            {
                throw new Exception("One way journey button not displayed", ex);
            }
        }
        private void SelectMulticityFlight()
        {
            try
            {
                WaitAndGetBySelector("flightJourneyTypeMulticity", ApplicationSettings.TimeOut.Fast).Click();
            }
            catch (Exception ex)
            {
                throw new Exception("Multicity journey button not displayed", ex);
            }
        }
        private void SelectRoundTripFlight()
        {
            try
            {
                WaitAndGetBySelector("flightJourneyTypeRoundTrip", ApplicationSettings.TimeOut.Fast).Click();
            }
            catch (Exception ex)
            {
                throw new Exception("RoundTrip journey button not displayed", ex);
            }
        }

        internal void Search(AirSearchCriteria airSearchCriteria)
        {
            SelectFlightPanel();
            SelectCabinType(airSearchCriteria);
            SetFlightType(airSearchCriteria.SearchType);
            //check if return or one way
            EnterAirports(airSearchCriteria.SearchType, airSearchCriteria.AirportPairs);
            //Enter from/to airports
            EnterPassengerDetails(airSearchCriteria.Passengers);
            ApplyFilters(airSearchCriteria.Filters);
            WaitAndGetBySelector("btnAirSearch", ApplicationSettings.TimeOut.Fast).Click();
        }

        private void ApplyFilters(AirFilters filters)
        {
            ExecuteJavascript("$('.jCabinClass').siblings('div').find('.filter-option').text('"+filters.CabinType.ToString().Replace('_',' ')+"')");
            if(filters.IncludeNearByAirPorts)
                WaitAndGetBySelector("includeNearByAirports",ApplicationSettings.TimeOut.SuperFast).Click();
            if (filters.NonStopFlight)
                ExecuteJavascript("$('#ulNonStopFlights').find('[data-value=\"NS\"]').click()");
            if(filters.AirLines!=null && filters.AirLines.Count!=0)
                filters.AirLines.ForEach(x=>ExecuteJavascript("$('#ulAirlines').find('[data-text=\""+x+"\"]').click()"));
                
        }

        private void SelectCabinType(AirSearchCriteria airSearchCriteria)
        {
            
        }

        private void EnterPassengerDetails(Passengers passengers)
        {
            try
            {
                WaitAndGetBySelector("adults", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(passengers.Adults.ToString());
                WaitAndGetBySelector("children", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(passengers.Children.ToString());
                WaitAndGetBySelector("infants", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(passengers.Infants.ToString());
            }
            catch (Exception Ex)
            {
                throw new Exception("Error while entering passenger details", Ex);
            }
        }

        private void EnterAirports(SearchType searchType, List<AirportPair> airportPairs)
        {
            if (searchType == SearchType.MultiCity)
                InputMultiCityAirPorts(airportPairs);
            else
            {
                WaitAndGetBySelector("fromAp", ApplicationSettings.TimeOut.Slow).SendKeys(airportPairs[0].DepartureAirport);
                WaitAndGetBySelector("toAp", ApplicationSettings.TimeOut.Slow).SendKeys(airportPairs[0].ArrivalAirport);
                WaitAndGetBySelector("onwardDate", ApplicationSettings.TimeOut.Slow).SendKeys(airportPairs[0].DepartureDateTime.ToString("MM/dd/yyyy"));
                if (searchType == SearchType.RoundTrip)
                    WaitAndGetBySelector("returnDate", ApplicationSettings.TimeOut.Slow).SendKeys(airportPairs[1].DepartureDateTime.ToString("MM/dd/yyyy"));
            }

        }

        private void InputMultiCityAirPorts(List<AirportPair> airportPairs)
        {
            var i = 1;
            while (true)
            {
                ExecuteJavascript("$('#depAP" + i + "').val('" + airportPairs[i - 1].DepartureAirport + "')");
                ExecuteJavascript("$('#retAP" + i + "').val('" + airportPairs[i - 1].ArrivalAirport + "')");
                //WaitAndGetBySelector("$('.jsMDDepDate1')", ApplicationSettings.TimeOut.Slow).SendKeys(airportPairs[i].DepartureDateTime.ToString("MM/dd/yyyy"));

                //ExecuteJavascript("$('input[val-rule=MDDepartureDate" + i + "]').val('" + airportPairs[i - 1].DepartureDateTime.ToString("MM/dd/yyyy") + "')");//.val('" + airportPairs[i].DepartureDateTime.ToString("MM/dd/yyyy") + "')");
                if (i == airportPairs.Count)
                    break;
                WaitAndGetBySelector("addFlight", ApplicationSettings.TimeOut.Fast).Click();
                ++i;
            }
            i = 0;
            var dates= GetUIElements("dates").Skip(2).Take(airportPairs.Count).ToList();
            dates.ForEach(x => x.SendKeys(airportPairs[i++].DepartureDateTime.ToString("MM/dd/yyyy")));
            dates[0].Click();
        }

 
        #endregion
    }



}
