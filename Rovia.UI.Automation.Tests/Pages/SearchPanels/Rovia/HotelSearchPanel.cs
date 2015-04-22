namespace Rovia.UI.Automation.Tests.Pages.SearchPanels
{
    using System.Linq;
    using System.Threading;
    using AppacitiveAutomationFramework;
    using Criteria;
    using Exceptions;
    using ScenarioObjects;
    using Configuration;

    /// <summary>
    /// Rovia site specific hotel product search methods
    /// </summary>
    public class HotelSearchPanel : UIPage, ISearchPanel
    {

        #region Protected Members

        protected virtual void SetPassengerDetails(Passengers passengers)
        {
            WaitAndGetBySelector("adults", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(passengers.Adults + (passengers.Adults > 1 ? " Adults" : " Adult"));
            if (passengers.Children == 0)
                return;
            WaitAndGetBySelector("children", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(passengers.Children + (passengers.Children > 1 ? " Children" : " Child"));
            var i = 0;
            GetUIElements("divChildAgeHolder").ForEach(x => x.SelectFromDropDown(passengers.ChildrenAges[i++]));

        }

        protected void SetStayPeriod(StayPeriod stayPeriod)
        {
            ExecuteJavascript("$('input.checkInDate').attr('readonly', false);");
            WaitAndGetBySelector("checkInDate", ApplicationSettings.TimeOut.Slow).SendKeys(stayPeriod.CheckInDate.ToString("MM/dd/yyyy"));
            ExecuteJavascript("$('input.checkOutDate').attr('readonly', false);");
            WaitAndGetBySelector("checkOutDate", ApplicationSettings.TimeOut.Slow).SendKeys(stayPeriod.CheckOutDate.ToString("MM/dd/yyyy"));
        }
        
        protected void SetLocation(string shortlocation, string location)
        {
            var locationHolder = WaitAndGetBySelector("inpShortLocation", ApplicationSettings.TimeOut.Slow);
            locationHolder.Click();
            locationHolder.SendKeys(shortlocation);
            if (location == null) return;
            IUIWebElement autoSuggestBox;
            do
            {
                autoSuggestBox = WaitAndGetBySelector("autoSuggestBox", ApplicationSettings.TimeOut.Fast);
            } while (autoSuggestBox == null || !autoSuggestBox.Displayed);
            GetUIElements("autoSuggestOptions").First(x => (x.Displayed && x.Text.Equals(location))).Click();
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

        protected virtual void ApplyPreSearchFilters(PreSearchFilters preSearchFilters)
        {
            var filters = preSearchFilters as HotelPreSearchFilters;
            if (!string.IsNullOrEmpty(filters.StarRating))
                ExecuteJavascript("$('.jHotelRating').val(" + filters.StarRating + ")");
            if (!string.IsNullOrEmpty(filters.HotelName))
                WaitAndGetBySelector("txtHotelName", ApplicationSettings.TimeOut.Fast).SendKeys(filters.HotelName);
            if (filters.AdditionalPreferences != null && filters.AdditionalPreferences.Count != 0)
                filters.AdditionalPreferences.ForEach(x => ExecuteJavascript("$('#ulAdditionalPref').find('[data-value=\"" + x + "\"]').click()"));
        }

        protected void ResolveMultiLocationOptions()
        {
            var multiLocOption = WaitAndGetBySelector("multiLocOptionButton", ApplicationSettings.TimeOut.Fast);
            if (multiLocOption != null && multiLocOption.Displayed)
                multiLocOption.Click();
        }

        #endregion

        #region Public Mebmers

        /// <summary>
        /// Search for hotel product
        /// </summary>
        /// <param name="searchCriteria">Hotel Search Criteria Object</param>
        public void Search(SearchCriteria searchCriteria)
        {
            var hotelSearchCriteria = searchCriteria as HotelSearchCriteria;
            SelectSearchPanel();
            SetStayPeriod(hotelSearchCriteria.StayPeriod);
            SetLocation(hotelSearchCriteria.ShortLocation, hotelSearchCriteria.Location);

            SetPassengerDetails(hotelSearchCriteria.Passengers);
            ApplyPreSearchFilters(hotelSearchCriteria.Filters.PreSearchFilters);
            WaitAndGetBySelector("btnHotelSearch", ApplicationSettings.TimeOut.Slow).Click();
            ResolveMultiLocationOptions();
        }

        #endregion
    }
}
