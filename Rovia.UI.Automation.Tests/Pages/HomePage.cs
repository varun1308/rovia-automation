using System;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.Tests.Configuration;
using System.Threading;


namespace Rovia.UI.Automation.Tests.Pages
{
    public class HomePage : UIPage
    {
        private void CheckForErrors()
        {
            try
            {
                GetUIElements("divErrors").ForEach(x =>
                {
                    if (x.Displayed)
                    {
                        throw new Alert(x.Text);
                    }
                });
            }
            catch (OpenQA.Selenium.StaleElementReferenceException)
            {
                LogManager.GetInstance().LogWarning("Caught and Suppressed StaleElementReferenceException");
            }
        }

        internal void WaitForHomePage()
        {
            try
            {
                var divHome = WaitAndGetBySelector("divHome", ApplicationSettings.TimeOut.Safe);
                if (divHome != null && divHome.Displayed)
                    //SetCountry();
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
            var anchor = ApplicationSettings.Environment == "PROD"
                ? WaitAndGetBySelector("logOutProd", ApplicationSettings.TimeOut.Fast) : WaitAndGetBySelector("logOutQA", ApplicationSettings.TimeOut.Fast);
            return anchor != null && anchor.Displayed;
        }

        internal void GoToLoginPage()
        {
            var login = ApplicationSettings.Environment == "PROD"
               ? WaitAndGetBySelector("lnkLogInProd", ApplicationSettings.TimeOut.Fast) : WaitAndGetBySelector("lnkLogInQA", ApplicationSettings.TimeOut.Fast);
            login.Click();
        }

        internal void LogOut()
        {
            var logout = ApplicationSettings.Environment == "PROD"
                ? WaitAndGetBySelector("logOutProd", ApplicationSettings.TimeOut.Fast) : WaitAndGetBySelector("logOutQA", ApplicationSettings.TimeOut.Fast);
            logout.Click();
        }

        internal void Search(SearchCriteria searchCriteria)
        {
            SearchPanel.Search(searchCriteria);
            CheckForErrors();
        }

        public ISearchPanel SearchPanel { get; set; }

        public string GetTripsErrorUri()
        {
            var sessionId = ApplicationSettings.Environment == "PROD" ? WaitAndGetBySelector("sessionIdProd", ApplicationSettings.TimeOut.Fast).GetAttribute("title") :
                WaitAndGetBySelector("sessionIdQA", ApplicationSettings.TimeOut.Fast).GetAttribute("title");
            return string.IsNullOrEmpty(sessionId) ? string.Empty :
               ApplicationSettings.TripsErrorUri + "?sessionid=" + sessionId;
        }
    }
}
