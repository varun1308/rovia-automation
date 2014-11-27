using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public class AirResultFilters : UIPage, IResultFilters
    {
        #region IResultPage Members
        public void SetPostSearchFilters(PostSearchFilters postSearchFilters)
        {
            var airPostSearchFilters = postSearchFilters as AirPostSearchFilters;
            SetStops(airPostSearchFilters.Stop);
            SetPriceRange(airPostSearchFilters.PriceRange);
            SetTimeDuration(airPostSearchFilters.MaxTimeDurationDiff);
            SetTakeOffTime(airPostSearchFilters.TakeOffTimeRange);
            SetLandingTime(airPostSearchFilters.LandingTimeRange);
            SetCabinTypes(airPostSearchFilters.CabinTypes);
            SetAirlines(airPostSearchFilters.Airlines);
        }

        public void SetMatrix(string matrixField)
        {
            var divMatrixAirlines = GetUIElements("divMatrixAirlines");
            if (divMatrixAirlines.Exists(x => matrixField.Equals(x.GetAttribute("title")) && x.Displayed))
                divMatrixAirlines.ForEach(x =>
                {
                    if (matrixField.Equals(x.GetAttribute("title")) && x.Displayed)
                        x.Click();
                });
            else
                throw new Exception("Expected airline not present in the results");

            if (!IsResultsAvailableOnFilter())
                Assert.IsTrue(VerifyMatrix(matrixField), "Matrix not applied.");
            else
                throw new Exception("Results not found for a given matrix");
        } 
        #endregion

        #region private Air Specific members
        private void SetPriceRange(PriceRange priceRange)
        {
            var minPrice =
                float.Parse(WaitAndGetBySelector("minPrice", ApplicationSettings.TimeOut.Fast).Text.Split(' ')[0].TrimStart('$'));
            var maxPrice =
               float.Parse(WaitAndGetBySelector("maxPrice", ApplicationSettings.TimeOut.Fast).Text.Split(' ')[0].TrimStart('$'));

            minPrice += minPrice * priceRange.Min / 100;
            maxPrice -= maxPrice * priceRange.Max / 100;

            ExecuteJavascript("$('#sliderRangePrice').trigger({type:'slideStop',value:[" + (minPrice * 100) + "," + (maxPrice * 100) + "]})");

            if (!IsResultsAvailableOnFilter())
                Assert.IsTrue(VerifyPriceFilter(minPrice, maxPrice), "Price filter not applied.");
            else
                throw new Exception("Results not found for a given price range");
        }

        private void SetTimeDuration(int maxTimeDurationCustom)
        {
            //taken out the max duration from slider
            var maxTimeDurationMins =
                WaitAndGetBySelector("maxTimeDuration", ApplicationSettings.TimeOut.Fast).Text.Split(' ');
            var maxTimeDuration = float.Parse(maxTimeDurationMins[0] + "." + maxTimeDurationMins[2] ?? "0");

            ExecuteJavascript("$('#sliderTripDuration').trigger({type:'slideStop',value:[" + (maxTimeDurationCustom * 60) + "]})");

            if (!IsResultsAvailableOnFilter())
                Assert.IsTrue(VerifyDurationFilter(maxTimeDurationCustom), "Duration filter not applied.");
            else
                throw new Exception("Results not found for a given duration");
        }

        private void SetStops(string stop)
        {
            var stops = GetUIElements("stopsFilter").ToList();
            stops.ForEach(x => x.Click());
            stops.ForEach(x =>
            {
                if (x.GetAttribute("data-name").Equals(stop))
                    x.Click();
            });
            if (IsResultsAvailableOnFilter())
                throw new Exception("Results not found for a given matrix");
                Assert.IsTrue(stop.Equals(VerifyStopsFilter()), "Stops filter not applied");
        }

        private string VerifyStopsFilter()
        {
            var legStops = GetUIElements("legCabinAndStop").Select(x =>
            {
                int num;
                int.TryParse(x.Text, out num);
                return num;
            }).Where((item, index) => index % 2 != 0).ToList();

            if (legStops.IndexOf(0) == -1)
            {
                if (legStops.IndexOf(1) == -1) return "one-plus";
                else return "one";
            }
            else return "none";
        }

        private void VerifyCabinTypesFilter(List<string> cabinTypes)
        {
            //to do implement
            var legStops = GetUIElements("legCabinAndStop").Select(x => x.Text.Split()[0]).Where((item, index) => index % 2 == 0).ToList();
            legStops.ForEach(x =>
            {
                if (cabinTypes.Contains(x)) return;
            });
        }

        private void SetCabinTypes(List<string> cabinTypes)
        {
            var cabinTypeList = GetUIElements("cabinTypeFilter").ToList();
            cabinTypeList[0].Click();
            cabinTypeList.ForEach(x =>
            {
                if (cabinTypes.Contains(x.GetAttribute("data-name")) && x.Displayed)
                    x.Click();
            });
            if (IsResultsAvailableOnFilter())
                throw new Exception("Results not found for a given matrix");
        }

        private void SetAirlines(List<string> airlines)
        {
            var airlinesList = GetUIElements("airlinesFilter").ToList();
            airlinesList[0].Click();
            airlinesList.ForEach(x =>
            {
                if (airlines.Contains(x.GetAttribute("data-code")) && x.Displayed)
                    x.Click();
            });

            if (IsResultsAvailableOnFilter())
                throw new Exception("Results not found for a given matrix");
        }

        private void SetTakeOffTime(TakeOffTimeRange takeOffTimeRange)
        {
            var jsTslider = WaitAndGetBySelector("jsTslider", ApplicationSettings.TimeOut.Fast);
            if (jsTslider != null && jsTslider.Displayed)
                ExecuteJavascript("var maxTime=parseInt($('.jsTslider').data('slider').max);" +
                                  "var minTime =maxTime- maxTime * " + takeOffTimeRange.Max + " / 100;maxTime -= maxTime * " + takeOffTimeRange.Min + " / 100;" +
                                  "$('.jsTslider').trigger({type:'slide',value:[minTime,maxTime]}).trigger({type:'slideStop',value:[minTime,maxTime]})");
            if (IsResultsAvailableOnFilter())
                throw new Exception("Results not found for a given matrix");
        }

        private void SetLandingTime(LandingTimeRange landingTimeRange)
        {
            var jsLslider = WaitAndGetBySelector("jsLslider", ApplicationSettings.TimeOut.Fast);
            if (jsLslider != null && jsLslider.Displayed)
                ExecuteJavascript("var maxTime=parseInt($('.jsLslider').data('slider').max);" +
                                      "var minTime =maxTime- maxTime * " + landingTimeRange.Max + " / 100;maxTime -= maxTime * " + landingTimeRange.Min + " / 100;" +
                                  "$('.jsLslider').trigger({type:'slide',value:[minTime,maxTime]}).trigger({type:'slideStop',value:[minTime,maxTime]})");
            if (IsResultsAvailableOnFilter())
                throw new Exception("Results not found for a given matrix");
        }

        private bool VerifyPriceFilter(float minSliderPrice, float maxSliderPrice)
        {
            return minSliderPrice <= GetFirstItineraryPrice("asc") && maxSliderPrice >= GetFirstItineraryPrice("desc");
        }

        private float GetFirstItineraryPrice(string sortBy)
        {
            ExecuteJavascript(sortBy.ToLower().Equals("asc") ? "$('#Price-Asc').click()" : "$('#Price-Desc').click()");
            Thread.Sleep(3000);
            return float.Parse(GetUIElements("amount").Select(x => x.Text.Split()[0].TrimStart('$')).ToArray()[0]);
        }

        private bool VerifyDurationFilter(float maxSliderDuration)
        {
            return maxSliderDuration >= GetMaxDurationFlight();
        }

        private float GetMaxDurationFlight()
        {
            ExecuteJavascript("$('#Duration-Desc').click()");
            Thread.Sleep(3000);
            var legDuration = GetUIElements("legDuration").Select(x =>
            {
                var arr = x.Text.Split();
                return (arr[1] + "." + (arr[3] ?? "0"));
            }).ToArray()[0];
            return float.Parse(legDuration);
        }

        private bool VerifyMatrix(string airline)
        {
            var titleAirLines = GetUIElements("titleAirLines");
            if (titleAirLines[0].Text.Equals(airline))
                return true;
            return false;
        }

        private bool IsResultsAvailableOnFilter()
        {
            var resultDiv = WaitAndGetBySelector("zeroAiritineraryDiv", ApplicationSettings.TimeOut.Slow);
            return resultDiv != null && resultDiv.Displayed;
        }
        #endregion
    }
}
