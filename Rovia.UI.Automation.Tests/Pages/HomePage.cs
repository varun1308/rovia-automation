using System;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Tests.Configuration;
using System.Threading;


namespace Rovia.UI.Automation.Tests.Pages
{
    public class HomePage : UIPage
    {
        private void CheckForErrors()
        {
            GetUIElements("divErrors").ForEach(x =>
            {
                if (x.Displayed)
                {
                    throw new Alert(x.Text);
                }
            });
        }

        internal void WaitForHomePage()
        {
            try
            {
                var divHome = WaitAndGetBySelector("divHome", ApplicationSettings.TimeOut.Safe);
                if (divHome != null && divHome.Displayed)
                    SetCountry();
                while (!IsVisible()) { }
            }
            catch (Exception exception)
            {
                throw new PageLoadFailed("HomePage", exception);
            }
        }

        internal bool IsVisible()
        {
            var div = WaitAndGetBySelector("divNavigationBar", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }

        internal void SetCountry()
        {
            var country = WaitAndGetBySelector("country", ApplicationSettings.TimeOut.Safe);
            Thread.Sleep(500);
            if (country != null && country.Displayed)
            {
                country.SelectFromDropDown("United States");
                WaitAndGetBySelector("saveCountry", ApplicationSettings.TimeOut.Slow).Click();
            }
        }

        internal bool IsUserLoggedIn()
        {
            var anchor = WaitAndGetBySelector("logOut", ApplicationSettings.TimeOut.Slow);
            return anchor != null && anchor.Displayed;
        }

        internal void GoToLoginPage()
        {
            WaitAndGetBySelector("lnkLogIn", ApplicationSettings.TimeOut.Fast).Click();
        }

        internal void LogOut()
        {
            WaitAndGetBySelector("logOut", ApplicationSettings.TimeOut.Fast).Click();
        }

        internal void Search(SearchCriteria searchCriteria)
        {
            SearchPanel.Search(searchCriteria);
            CheckForErrors();
        }

        public SearchPanel SearchPanel { get; set; }

        public string GetSessionId()
        {
            return WaitAndGetBySelector("sessionId", ApplicationSettings.TimeOut.Fast).GetAttribute("title").ToString();
        }
    }
}
