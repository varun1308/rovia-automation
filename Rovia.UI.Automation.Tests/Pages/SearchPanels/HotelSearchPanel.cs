using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.SearchPanels
{
    public class HotelSearchPanel:SearchPanel
    {

        private void SetPassengerDetails(Passengers passengers)
        {
            try
            {
                WaitAndGetBySelector("adults", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(passengers.Adults.ToString());
                if (passengers.Children==0)
                    return;
                WaitAndGetBySelector("children", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(passengers.Children.ToString());
                var i = 0;
                GetUIElements("divChildAgeHolder").ForEach(x=>x.SelectFromDropDown(passengers.ChildrenAges[i++]));
            }
            catch (Exception ex)
            {
                throw new Exception("Error while entering passenger details", ex);
            }
        }

        private void SetStayPeriod(StayPeriod stayPeriod)
        {
            WaitAndGetBySelector("checkInDate", ApplicationSettings.TimeOut.Slow).SendKeys(stayPeriod.CheckInDate.ToString("MM/dd/yyyy"));
            WaitAndGetBySelector("checkOutDate", ApplicationSettings.TimeOut.Slow).SendKeys(stayPeriod.CheckOutDate.ToString("MM/dd/yyyy"));           
        }

        private void SetLocation(string location)
        {
            WaitAndGetBySelector("inpLocation", ApplicationSettings.TimeOut.Slow).SendKeys(location);
        }

        protected override void ApplyPreSearchFilters(PreSearchFilters preSearchFilters)
        {
            var filters = preSearchFilters as HotelPreSearchFilters;
            ExecuteJavascript("$(.jHotelRating').val("+filters.StarRating+")");
            if (string.IsNullOrEmpty(filters.HotelName))
                WaitAndGetBySelector("txtHotelName",ApplicationSettings.TimeOut.Fast).SendKeys(filters.HotelName);
            if (filters.AdditionalPreferences != null && filters.AdditionalPreferences.Count != 0)
                filters.AdditionalPreferences.ForEach(x => ExecuteJavascript("$('#ulAdditionalPref').find('[data-value=\""+x+"\"]').click()"));
        }
        public override void Search(SearchCriteria searchCriteria)
        {
            try
            {
                var hotelSearchCriteria = searchCriteria as HotelSearchCriteria;
                SelectSearchPanel();
                SetLocation(hotelSearchCriteria.Location);
                SetStayPeriod(hotelSearchCriteria.StayPeriod);
                SetPassengerDetails(hotelSearchCriteria.Passengers);
                ApplyPreSearchFilters(hotelSearchCriteria.Filters.PreSearchFilters);
                WaitAndGetBySelector("btnHotelSearch",ApplicationSettings.TimeOut.Fast).Click();
            }
            catch (Exception exception)
            {
                throw new Exception("Hotel Search Failed",exception);
            }
        }

    }
}
