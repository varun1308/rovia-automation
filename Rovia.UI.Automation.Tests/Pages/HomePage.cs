using System;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Model;
using Rovia.UI.Automation.Tests.Utility;
using System.Threading;

namespace Rovia.UI.Automation.Tests.Pages
{
    public class HomePage : UIPage
    {
        internal bool IsVisible()
        {
            var div = WaitAndGetBySelector("divHome", ApplicationSettings.TimeOut.Safe);
            //for selecting country
            var country = WaitAndGetBySelector("country", ApplicationSettings.TimeOut.Safe);
            if (country != null && country.Displayed)
            {
                country.SendKeys("United States");
                var saveCountry = WaitAndGetBySelector("saveCountry", ApplicationSettings.TimeOut.SuperFast);
                saveCountry.Click();
            }
            return div != null && div.Displayed;
        }

        internal void DoAirSearch(AirSearchScenario airScenario)
        {
            var flights = WaitAndGetBySelector("flights", ApplicationSettings.TimeOut.Slow);
            if (flights == null || !flights.Displayed)
            {
                Assert.Fail("Flights Nav Bar not displayed.");
            }
            flights.Click();

            var flightPanel = WaitAndGetBySelector("flightsSearchPanel", ApplicationSettings.TimeOut.Safe);

            if(flightPanel==null || !flightPanel.Displayed)
                Assert.Fail("Flights Panel not displayed.");
            
            //check if journey is multicity
            if (airScenario.SearchType == SearchType.Multicity)
            {
                var btnMulticity = WaitAndGetBySelector("flightJourneyTypeMulticity", ApplicationSettings.TimeOut.Fast);

                if (btnMulticity == null || !btnMulticity.Displayed)
                    Assert.Fail("Multicity journey button not displayed.");
                btnMulticity.Click();
                for (int i = 0; i < airScenario.AirportPairs.Count; i++)
                {
                    ExecuteJavascript("$('#depAP"+(i+1)+"').val('"+airScenario.AirportPairs[i].FromLocation+"')");
                    ExecuteJavascript("$('#retAP" + (i + 1) + "').val('" + airScenario.AirportPairs[i].ToLocation + "')");
                    ExecuteJavascript("$('input[val-rule=\"MDDepartureDate" + (i + 1) + "\"]').val('" + airScenario.AirportPairs[i].DepartureDateTime.ToString("MM/dd/yyyy") + "')");

                    if (i < airScenario.AirportPairs.Count-1)
                    {
                        WaitAndGetBySelector("btnaddflight", ApplicationSettings.TimeOut.Fast).Click();
                    }
                }
            }
            else
            {
                //check if return or one way
                if (airScenario.SearchType == SearchType.OneWay)
                {
                    var btnOneway = WaitAndGetBySelector("flightJourneyTypeOneWay", ApplicationSettings.TimeOut.Fast);

                    if (btnOneway == null || !btnOneway.Displayed)
                        Assert.Fail("One way journey button not displayed.");

                    btnOneway.Click();
                }

                //Enter from/to airports

                var input = WaitAndGetBySelector("fromAp", ApplicationSettings.TimeOut.Fast);

                input.SendKeys(airScenario.AirportPairs[0].FromLocation);

                input = WaitAndGetBySelector("toAp", ApplicationSettings.TimeOut.Fast);
                input.SendKeys(airScenario.AirportPairs[0].ToLocation);

                //enter date

                input = WaitAndGetBySelector("onwardDate", ApplicationSettings.TimeOut.Fast);

                input.SendKeys(airScenario.AirportPairs[0].DepartureDateTime.ToString("MM/dd/yyyy"));

                if (airScenario.SearchType == SearchType.Return)
                {
                    input = WaitAndGetBySelector("returnDate", ApplicationSettings.TimeOut.Fast);

                    input.SendKeys(airScenario.AirportPairs[1].DepartureDateTime.ToString("MM/dd/yyyy"));
                }
            }

            WaitAndGetBySelector("adults", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(airScenario.Adults.ToString());
            WaitAndGetBySelector("children", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(airScenario.Childs.ToString());
            WaitAndGetBySelector("infants", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(airScenario.Infants.ToString());

            WaitAndGetBySelector("btnSearch", ApplicationSettings.TimeOut.Fast).Click();
        }


    }



}
