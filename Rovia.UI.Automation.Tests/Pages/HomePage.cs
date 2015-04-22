namespace Rovia.UI.Automation.Tests.Pages
{
    using AppacitiveAutomationFramework;
    using Configuration;
    using Criteria;
    using Exceptions;
    using Logger;
    using System;
    using System.Threading;

    /// <summary>
    /// This class contains all the fields and methods for site home page
    /// </summary>
    public class HomePage : UIPage
    {
        #region Private Members

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

        private bool IsVisible()
        {
            var div = WaitAndGetBySelector("divNavigationBar", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }

        #endregion

        #region Public Properties

        public ISearchPanel SearchPanel { get; set; }

        #endregion

        #region Public Members

        /// <summary>
        /// Wait for home page to load
        /// </summary>
        public void WaitForHomePage()
        {
            try
            {
                var divHome = WaitAndGetBySelector("divHome", ApplicationSettings.TimeOut.Safe);
                if (divHome != null && divHome.Displayed) { }
                while (!IsVisible()) { }
            }
            catch (Exception exception)
            {
                throw new PageLoadFailed("HomePage", exception);
            }
        }

        /// <summary>
        /// Used only in case of dev env, ignored for QA/Prod env
        /// </summary>
        public void SetCountry()
        {
            var country = WaitAndGetBySelector("country", ApplicationSettings.TimeOut.Safe);
            Thread.Sleep(500);
            if (country != null && country.Displayed)
            {
                country.SelectFromDropDown("United States");
                WaitAndGetBySelector("saveCountry", ApplicationSettings.TimeOut.Slow).Click();
            }
        }

        /// <summary>
        /// Redirect to log in page
        /// </summary>
        public void GoToLoginPage()
        {
            IUIWebElement login;
            switch (ApplicationSettings.Environment)
            {
                case "QA":
                    login = WaitAndGetBySelector("lnkLogInQA", ApplicationSettings.TimeOut.Fast);
                    break;
                case "STAGE":
                    login = WaitAndGetBySelector("lnkLogInStage", ApplicationSettings.TimeOut.Fast);
                    break;
                case "PROD":
                    login = WaitAndGetBySelector("lnkLogInProd", ApplicationSettings.TimeOut.Fast);
                    break;
                default:
                    login = WaitAndGetBySelector("lnkLogInPreQA", ApplicationSettings.TimeOut.Fast);
                    break;
            }
            login.Click();
        }

        /// <summary>
        /// Log out from site
        /// </summary>
        public void LogOut()
        {
            IUIWebElement logout;
            switch (ApplicationSettings.Environment)
            {
                case "QA":
                    logout = WaitAndGetBySelector("logOutQA", ApplicationSettings.TimeOut.Fast);
                    break;
                case "STAGE":
                    logout = WaitAndGetBySelector("logOutStage", ApplicationSettings.TimeOut.Fast);
                    break;
                case "PROD":
                    logout = WaitAndGetBySelector("logOutProd", ApplicationSettings.TimeOut.Fast);
                    break;
                default:
                    logout = WaitAndGetBySelector("logOutPreQA", ApplicationSettings.TimeOut.Fast);
                    break;
            }
            logout.Click();
        }

        /// <summary>
        /// Initiate search action
        /// </summary>
        /// <param name="searchCriteria"></param>
        public void Search(SearchCriteria searchCriteria)
        {
            SearchPanel.Search(searchCriteria);
            CheckForErrors();
        }

        /// <summary>
        /// Get trips error UI URL with sessionId
        /// </summary>
        /// <returns></returns>
        public string GetTripsErrorUri()
        {

            string sessionId;
            switch (ApplicationSettings.Environment)
            {
                case "QA":
                    sessionId =
                        WaitAndGetBySelector("sessionIdQA", ApplicationSettings.TimeOut.Fast).GetAttribute("title");
                    break;
                case "STAGE":
                    sessionId = WaitAndGetBySelector("sessionIdStage", ApplicationSettings.TimeOut.Fast).GetAttribute("title");
                    break;
                case "PROD":
                    sessionId = WaitAndGetBySelector("sessionIdProd", ApplicationSettings.TimeOut.Fast).GetAttribute("title");
                    break;
                default:
                    sessionId = WaitAndGetBySelector("sessionIdPreQA", ApplicationSettings.TimeOut.Fast).GetAttribute("title");
                    break;
            }
            return string.IsNullOrEmpty(sessionId) ? string.Empty :
                ApplicationSettings.TripsErrorUri + "?sessionId=" + sessionId;
        }

        #endregion
    }
}
