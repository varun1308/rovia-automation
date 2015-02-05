namespace Rovia.UI.Automation.Tests.Pages.SearchPanels
{
    using System.Linq;
    using ScenarioObjects;
    using Configuration;

    /// <summary>
    /// Travel site specific hotel product search methods
    /// </summary>
    public class TravelHotelSearchPanel : HotelSearchPanel
    {
        /// <summary>
        /// Override enter passenger details method with respect to travel site controls
        /// </summary>
        /// <param name="passengers"></param>
        protected override void SetPassengerDetails(Passengers passengers)
        {
            GetUIElements("dropdownClick").FirstOrDefault(x => x.Text.Contains("Adults")).Click();
            GetUIElements("dropdownSelect").FirstOrDefault(x => x.Text.Equals(passengers.Adults + (passengers.Adults > 1 ? " Adults" : " Adult"))).Click();
            if (passengers.Children == 0)
                return;
            GetUIElements("dropdownClick").FirstOrDefault(x => x.Text.Contains("Child")).Click();
            GetUIElements("dropdownSelect").FirstOrDefault(x => x.Text.Equals(passengers.Children + (passengers.Children > 1 ? " Children" : " Child"))).Click();
            
            var i = 0;
            GetUIElements("divChildAgeHolder").ForEach(x => x.SelectFromDropDown(passengers.ChildrenAges[i++]));

        }

        /// <summary>
        /// Override applying pre serach filters method with respect to travel site controls
        /// </summary>
        /// <param name="preSearchFilters"></param>
        protected override void ApplyPreSearchFilters(PreSearchFilters preSearchFilters)
        {
            var filters = preSearchFilters as HotelPreSearchFilters;
            if (!string.IsNullOrEmpty(filters.StarRating))
            {
                GetUIElements("dropdownClick").FirstOrDefault(x => x.Text.Contains("Star")).Click();
                GetUIElements("dropdownSelect").FirstOrDefault(x => x.Text.Contains(filters.StarRating)).Click();
            }
            if (!string.IsNullOrEmpty(filters.HotelName))
                WaitAndGetBySelector("txtHotelName", ApplicationSettings.TimeOut.Fast).SendKeys(filters.HotelName);
            if (filters.AdditionalPreferences != null && filters.AdditionalPreferences.Count != 0)
                filters.AdditionalPreferences.ForEach(x => ExecuteJavascript("$('#ulAdditionalPref').find('[data-value=\"" + x + "\"]').click()"));
        }
    }
}
