using System;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.SearchPanels
{
    class ActivitySearchPanel : UIPage, ISearchPanel
    {
        private void SelectSearchPanel()
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

        private void SetLocation(string shortlocation, string location)
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

        public void Search(SearchCriteria searchCriteria)
        {
            var activitySearchCriteria = searchCriteria as ActivitySearchCriteria;
            SelectSearchPanel();
            WaitAndGetBySelector("fromDate", ApplicationSettings.TimeOut.Slow).SendKeys(activitySearchCriteria.FromDate.ToString("MM/dd/yyyy"));
            WaitAndGetBySelector("toDate", ApplicationSettings.TimeOut.Slow).SendKeys(activitySearchCriteria.ToDate.ToString("MM/dd/yyyy"));
            SetLocation(activitySearchCriteria.ShortLocation, activitySearchCriteria.Location);
            WaitAndGetBySelector("btnActivitySearch", ApplicationSettings.TimeOut.Slow).Click();
        }

    }
}
