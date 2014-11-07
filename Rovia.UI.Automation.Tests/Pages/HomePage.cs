using System;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Pages
{
    public class HomePage : UIPage
    {
        internal bool IsVisible()
        {
            var div = WaitAndGetBySelector("divHome", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }

        internal void DoAirSearch(string from, string to, DateTime onwardDateTime, bool isReturn, DateTime returnDateTime,
            int adults=1, int children=0, int infants=0)
        {
            var flights = WaitAndGetBySelector("flights", ApplicationSettings.TimeOut.Slow);

            if (flights == null || !flights.Displayed)
            {
                Assert.Fail("Flights Nav Bar not displayed.");
            }

            flights.Click();

            var flightPanel = WaitAndGetBySelector("flightsSearchPanel", ApplicationSettings.TimeOut.Fast);

            if(flightPanel==null || !flightPanel.Displayed)
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

            var input = WaitAndGetBySelector("fromAp", ApplicationSettings.TimeOut.Fast);

            input.SendKeys(from);

            input = WaitAndGetBySelector("toAp", ApplicationSettings.TimeOut.Fast);
            input.SendKeys(to);

            //enter date

            input = WaitAndGetBySelector("onwardDate", ApplicationSettings.TimeOut.Fast);
            
            input.SendKeys(onwardDateTime.ToString("MM/dd/yyyy"));

            if (isReturn)
            {
                input = WaitAndGetBySelector("returnDate", ApplicationSettings.TimeOut.Fast);

                input.SendKeys(onwardDateTime.ToString("MM/dd/yyyy"));
            }

            var combo = WaitAndGetBySelector("adults", ApplicationSettings.TimeOut.Fast);

            combo.SelectFromDropDown(adults.ToString());

            combo = WaitAndGetBySelector("children", ApplicationSettings.TimeOut.Fast);

            combo.SelectFromDropDown(children.ToString());

            combo = WaitAndGetBySelector("infants", ApplicationSettings.TimeOut.Fast);

            combo.SelectFromDropDown(infants.ToString());

            var btnSearch = WaitAndGetBySelector("btnSearch", ApplicationSettings.TimeOut.Fast);

            btnSearch.Click();
        }


    }



}
