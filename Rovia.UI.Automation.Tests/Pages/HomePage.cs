using System;
using System.Collections.Generic;
using System.Linq;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;
using System.Threading;


namespace Rovia.UI.Automation.Tests.Pages
{
    public class HomePage : UIPage
    {
        
        #region Common
        internal void WaitForHomePage()
        {
            try
            {
                SetCountry();
                while (!IsVisible());
                
            }
            catch (Exception exception)
            {
                throw new Exception("HomePage failed to load", exception);
            }
        }
        internal bool IsVisible()
        {
            var div = WaitAndGetBySelector("divNavigationBar", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }

        internal void SetCountry()
        {
            var country = WaitAndGetBySelector("country", ApplicationSettings.TimeOut.Slow);
            if (country == null || !country.Displayed) return;
            country.SendKeys("United States");
            WaitAndGetBySelector("saveCountry", ApplicationSettings.TimeOut.Slow).Click();
        }

        internal bool IsUserLoggedIn()
        {
            var anchor = WaitAndGetBySelector("logOut", ApplicationSettings.TimeOut.Slow);
            return anchor != null && anchor.Displayed;
        }

        internal void DoAirSearch(string from, string to, DateTime onwardDateTime, bool isReturn, DateTime returnDateTime,
        int adults = 1, int children = 0, int infants = 0)
        {
            var flights = WaitAndGetBySelector("flights", ApplicationSettings.TimeOut.Slow);

            if (flights == null || !flights.Displayed)
            {
                Assert.Fail("Flights Nav Bar not displayed.");
            }

            flights.Click();

            Thread.Sleep(500);
            var flightPanel = WaitAndGetBySelector("flightsSearchPanel", ApplicationSettings.TimeOut.Slow);

            if (flightPanel == null || !flightPanel.Displayed)
                Assert.Fail("Flights Panel not displayed.");

            //check if return or one way
            if (!isReturn)
            {
                var btnOneway = WaitAndGetBySelector("flightJourneyTypeOneWay", ApplicationSettings.TimeOut.Fast);

                if (btnOneway == null || !btnOneway.Displayed)
                    Assert.Fail("One way journey button not displayed.");

                btnOneway.Click();
            }

            //Enter from/to airports

            var input = WaitAndGetBySelector("fromAp", ApplicationSettings.TimeOut.Slow);

            input.SendKeys(from);

            input = WaitAndGetBySelector("toAp", ApplicationSettings.TimeOut.Slow);
            input.SendKeys(to);

            //enter date

            input = WaitAndGetBySelector("onwardDate", ApplicationSettings.TimeOut.Slow);

            input.SendKeys(onwardDateTime.ToString("MM/dd/yyyy"));

            if (isReturn)
            {
                input = WaitAndGetBySelector("returnDate", ApplicationSettings.TimeOut.Slow);

                input.SendKeys(onwardDateTime.ToString("MM/dd/yyyy"));
            }

            var combo = WaitAndGetBySelector("adults", ApplicationSettings.TimeOut.Fast);

            combo.SelectFromDropDown(adults.ToString());

            combo = WaitAndGetBySelector("children", ApplicationSettings.TimeOut.Fast);

            combo.SelectFromDropDown(children.ToString());

            combo = WaitAndGetBySelector("infants", ApplicationSettings.TimeOut.Fast);

            combo.SelectFromDropDown(infants.ToString());

            var btnSearch = WaitAndGetBySelector("btnAirSearch", ApplicationSettings.TimeOut.Fast);

            btnSearch.Click();
        }



        internal void GoToLoginPage()
        {
            WaitAndGetBySelector("lnkLogIn", ApplicationSettings.TimeOut.Fast).Click();
        }

        internal void LogOut()
        {
            WaitAndGetBySelector("logOut", ApplicationSettings.TimeOut.Fast).Click();
        }
        #endregion

        internal void Search(SearchCriteria searchCriteria)
        {
            SearchPanel.Search(searchCriteria);
        }

        public SearchPanel SearchPanel { get; set; }
    }



}
