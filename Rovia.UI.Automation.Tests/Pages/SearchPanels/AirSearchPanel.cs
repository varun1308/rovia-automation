using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.SearchPanels
{
    public class AirSearchPanel:SearchPanel
    {
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
                throw new UIElementNullOrNotVisible("One-way journey button", ex);
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
                throw new UIElementNullOrNotVisible("Multicity journey button", ex);
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
                throw new UIElementNullOrNotVisible("RoundTrip journey button", ex);
            }
        }
        protected override void ApplyPreSearchFilters(PreSearchFilters preSearchFilters)
        {
            var filters = preSearchFilters as AirPreSearchFilters;
            ExecuteJavascript("$('.jCabinClass').siblings('div').find('.filter-option').text('" + filters.CabinType.ToString().Replace('_', ' ') + "')");
            if (filters.IncludeNearByAirPorts)
                WaitAndGetBySelector("includeNearByAirports", ApplicationSettings.TimeOut.SuperFast).Click();
            if (filters.NonStopFlight)
                ExecuteJavascript("$('#ulNonStopFlights').find('[data-value=\"NS\"]').click()");
            if (filters.AirLines != null && filters.AirLines.Count != 0)
                filters.AirLines.ForEach(x => ExecuteJavascript("$('#ulAirlines').find('[data-text=\"" + x + "\"]').click()"));

        }
        private void EnterPassengerDetails(Passengers passengers)
        {
            try
            {
                WaitAndGetBySelector("adults", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(passengers.Adults.ToString());
                WaitAndGetBySelector("children", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(passengers.Children.ToString());
                WaitAndGetBySelector("infants", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(passengers.Infants.ToString());
            }
            catch (Exception ex)
            {
                LogManager.GetInstance().LogInformation("Error While Entering Passenger Details");
                throw;// new Exception("Error while entering passenger details", ex);
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

        public override void Search(SearchCriteria searchCriteria)
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
            
        }
    }
}
