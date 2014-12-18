using System;
using System.Linq;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.SearchPanels
{
    public class HotelSearchPanel:SearchPanel
    {

        private void SetPassengerDetails(Passengers passengers)
        {
            
                WaitAndGetBySelector("adults", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(passengers.Adults+(passengers.Adults>1?" Adults":" Adult"));
                if (passengers.Children==0)
                    return;
                WaitAndGetBySelector("children", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(passengers.Children + (passengers.Children > 1 ? " Children" : " Child"));
                var i = 0;
                GetUIElements("divChildAgeHolder").ForEach(x=>x.SelectFromDropDown(passengers.ChildrenAges[i++]));
            
        }

        private void SetStayPeriod(StayPeriod stayPeriod)
        {
            WaitAndGetBySelector("checkInDate", ApplicationSettings.TimeOut.Slow).SendKeys(stayPeriod.CheckInDate.ToString("MM/dd/yyyy"));
            WaitAndGetBySelector("checkOutDate", ApplicationSettings.TimeOut.Slow).SendKeys(stayPeriod.CheckOutDate.ToString("MM/dd/yyyy"));           
        }

        private void SetLocation(string shortlocation,string location)
        {
            WaitAndGetBySelector("inpShortLocation", ApplicationSettings.TimeOut.Slow).SendKeys(shortlocation);
            if (location != null)
            {
                IUIWebElement autoSuggestBox;
                do
                {
                    autoSuggestBox = WaitAndGetBySelector("autoSuggestBox", ApplicationSettings.TimeOut.Fast);
                } while (autoSuggestBox == null || !autoSuggestBox.Displayed);
                GetUIElements("autoSuggestOptions").First(x => (x.Displayed && x.Text.Equals(location))).Click();
            }
        }

        protected override void ApplyPreSearchFilters(PreSearchFilters preSearchFilters)
        {
            var filters = preSearchFilters as HotelPreSearchFilters;
            if (!string.IsNullOrEmpty(filters.StarRating))
                ExecuteJavascript("$('.jHotelRating').val(" + filters.StarRating +")"); 
            if (!string.IsNullOrEmpty(filters.HotelName))
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
                SetStayPeriod(hotelSearchCriteria.StayPeriod);
                SetLocation(hotelSearchCriteria.ShortLocation, hotelSearchCriteria.Location);

                SetPassengerDetails(hotelSearchCriteria.Passengers);
                ApplyPreSearchFilters(hotelSearchCriteria.Filters.PreSearchFilters);
                WaitAndGetBySelector("btnHotelSearch", ApplicationSettings.TimeOut.Slow).Click();
            }
            catch (Exception exception)
            {
                
                throw;
            }
        }

    }
}
