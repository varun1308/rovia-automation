using System.Linq;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.SearchPanels
{
    public class TravelHotelSearchPanel : HotelSearchPanel
    {
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
