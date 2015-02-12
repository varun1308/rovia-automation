using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Framework.Configurations;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Framework.Pages
{
    /// <summary>
    /// Rovia site specific air product search methods
    /// </summary>
    public class AirSearchPanel : UIPage , ISearchPanel
    {
        #region Protected Members

        protected void SetFlightType(SearchType searchType)
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

        protected void SelectOneWayFlight()
        {
            try
            {
                WaitAndGetBySelector("flightJourneyTypeOneWay", ApplicationSettings.TimeOut.Fast).Click();
            }
            catch (Exception ex)
            {
                throw new UIElementNullOrNotVisible("One-way journey button", ex);
            }
        }

        protected void SelectMulticityFlight()
        {
            try
            {
                WaitAndGetBySelector("flightJourneyTypeMulticity", ApplicationSettings.TimeOut.Fast).Click();
            }
            catch (Exception ex)
            {
                throw new UIElementNullOrNotVisible("Multicity journey button", ex);
            }
        }

        protected void SelectRoundTripFlight()
        {
            try
            {
                WaitAndGetBySelector("flightJourneyTypeRoundTrip", ApplicationSettings.TimeOut.Fast).Click();
            }
            catch (Exception ex)
            {
                throw new UIElementNullOrNotVisible("RoundTrip journey button", ex);
            }
        }

        protected void EnterPassengerDetails(Passengers passengers)
        {
            try
            {
                WaitAndGetBySelector("adults", ApplicationSettings.TimeOut.Slow).SelectFromDropDown(passengers.Adults.ToString());
                WaitAndGetBySelector("children", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(passengers.Children.ToString());
                WaitAndGetBySelector("infants", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(passengers.Infants.ToString());
            }
            catch (Exception)
            {
                LogManager.GetInstance().LogInformation("Error While Entering Passenger Details");
                throw;
            }
        }

        protected void ResolveMultiLocationOptions()
        {
            var multiLocOption = WaitAndGetBySelector("multiLocOptionButton", ApplicationSettings.TimeOut.Fast);
            if (multiLocOption != null && multiLocOption.Displayed)
                multiLocOption.Click();
        }

        protected void EnterAirports(SearchType searchType, List<AirportPair> airportPairs)
        {
            if (searchType == SearchType.MultiCity)
                InputMultiCityAirPorts(airportPairs);
            else
            {
                WaitAndGetBySelector("fromAp", ApplicationSettings.TimeOut.Slow).SendKeys(airportPairs[0].DepartureAirport);
                WaitAndGetBySelector("toAp", ApplicationSettings.TimeOut.Slow).SendKeys(airportPairs[0].ArrivalAirport);
                WaitAndGetBySelector("onwardDate", ApplicationSettings.TimeOut.Slow).SendKeys(airportPairs[0].DepartureDateTime.ToString("MM/dd/yyyy"));
                if (searchType == SearchType.RoundTrip)
                    WaitAndGetBySelector("returnDate", ApplicationSettings.TimeOut.Slow).SendKeys(
                        airportPairs[1].DepartureDateTime.ToString("MM/dd/yyyy"));
                WaitAndGetBySelector("fromAp", ApplicationSettings.TimeOut.Slow).Click();
            }

        }

        protected void InputMultiCityAirPorts(List<AirportPair> airportPairs)
        {
            var i = 1;
            while (true)
            {
                ExecuteJavascript("$('#depAP" + i + "').val('" + airportPairs[i - 1].DepartureAirport + "')");
                ExecuteJavascript("$('#retAP" + i + "').val('" + airportPairs[i - 1].ArrivalAirport + "')");

                if (i == airportPairs.Count)
                    break;
                WaitAndGetBySelector("addFlight", ApplicationSettings.TimeOut.Fast).Click();
                ++i;
            }
            i = 0;
            var dates = GetUIElements("dates").Skip(2).Take(airportPairs.Count).ToList();
            dates.ForEach(x => x.SendKeys(airportPairs[i++].DepartureDateTime.ToString("MM/dd/yyyy")));
            dates[0].Click();
        }

        protected void SelectSearchPanel()
        {
            var navBar = WaitAndGetBySelector("navBar", ApplicationSettings.TimeOut.Slow);
            if (navBar == null || !navBar.Displayed)
                throw new UIElementNullOrNotVisible("Navigation Bar ");
            navBar.Click();

            Thread.Sleep(500);

            var searchPanel = WaitAndGetBySelector("searchPanel", ApplicationSettings.TimeOut.Slow);
            if (searchPanel == null || !searchPanel.Displayed)
                throw new UIElementNullOrNotVisible("SearchPanel");
        }

        protected void ApplyPreSearchFilters(PreSearchFilters preSearchFilters)
        {
            var filters = preSearchFilters as AirPreSearchFilters;
            if (!string.IsNullOrEmpty(filters.CabinType.ToString()))
            {
                IUIWebElement firstOrDefault = GetUIElements("cabinTypeClick").FirstOrDefault(x => x.Text.Contains("Cabin") || x.Text.Contains("Economy"));
                if (firstOrDefault != null)
                    firstOrDefault.Click();
                var cabinPref = GetUIElements("cabinTypeOptions").ToList();
               cabinPref.ForEach(x => {
                   if (x.Text.Equals(filters.CabinType.ToString().Replace('_', ' '), StringComparison.InvariantCultureIgnoreCase))
                   {
                       x.Click();
                   }
               });//GetAttribute("innerText")
                //ExecuteJavascript("$('.jCabinClass').siblings('div').find('.filter-option').text('" + filters.CabinType.ToString().Replace('_', ' ') + "')");
            }
            if (filters.IncludeNearByAirPorts)
                WaitAndGetBySelector("includeNearByAirports", ApplicationSettings.TimeOut.SuperFast).Click();
            if (filters.NonStopFlight)
                ExecuteJavascript("$('#ulNonStopFlights').find('[data-value=\"NS\"]').click()");
            if (filters.AirLines != null && filters.AirLines.Count != 0)
                filters.AirLines.ForEach(x => ExecuteJavascript("$('#ulAirlines').find('[data-text=\"" + x + "\"]').click()"));

        }

        #endregion

        #region Public Members

        /// <summary>
        /// Search for air product
        /// </summary>
        /// <param name="searchCriteria">Air Search Criteria Object</param>
        public void Search(SearchCriteria searchCriteria)
        {
            var airSearchCriteria = searchCriteria as AirSearchCriteria;
            Thread.Sleep(4000);
            SelectSearchPanel();
            SetFlightType(airSearchCriteria.SearchType);
            //check if return or one way
            EnterAirports(airSearchCriteria.SearchType, airSearchCriteria.AirportPairs);
            //Enter from/to airports
            EnterPassengerDetails(airSearchCriteria.Passengers);
            ApplyPreSearchFilters(airSearchCriteria.Filters.PreSearchFilters as AirPreSearchFilters);
            WaitAndGetBySelector("btnAirSearch", ApplicationSettings.TimeOut.Slow).Click();
            ResolveMultiLocationOptions();
        }

        #endregion
    }
}
