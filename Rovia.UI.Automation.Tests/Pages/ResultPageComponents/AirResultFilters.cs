using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public class AirResultFilters : UIPage, IResultFilters
    {

        #region Private Members

        private bool SetPriceRange(PriceRange priceRange)
        {
            var minPrice =
                float.Parse(WaitAndGetBySelector("minPrice", ApplicationSettings.TimeOut.Fast).Text.Split(' ')[0].TrimStart('$'));
            var maxPrice =
               float.Parse(WaitAndGetBySelector("maxPrice", ApplicationSettings.TimeOut.Fast).Text.Split(' ')[0].TrimStart('$'));

            minPrice += minPrice * priceRange.Min / 100;
            maxPrice -= maxPrice * priceRange.Max / 100;

            ExecuteJavascript("$('#sliderRangePrice').trigger({type:'slideStop',value:[" + (minPrice * 100) + "," + (maxPrice * 100) + "]})");

            return IsPostResultsFilterApplied(new List<string> { "Price" });// && !IsResultsNotAvailableOnFilter();
        }

        private bool SetTimeDuration(int maxTimeDurationCustom)
        {
            //taken out the max duration from slider
            var maxTimeDurationMins =
                WaitAndGetBySelector("maxTimeDuration", ApplicationSettings.TimeOut.Fast).Text.Split(' ');
            var maxTimeDuration = float.Parse(maxTimeDurationMins[0] + "." + maxTimeDurationMins[2] ?? "0");

            ExecuteJavascript("$('#sliderTripDuration').trigger({type:'slideStop',value:[" + (maxTimeDurationCustom * 60) + "]})");

            return IsPostResultsFilterApplied(new List<string> { "Trip Duration" });// && !IsResultsNotAvailableOnFilter();
        }

        private bool SetStops(string stop)
        {
            var stops = GetUIElements("stopsFilter").ToList();
            stops.ForEach(x => x.Click());

            stops.ForEach(x =>
            {
                if (x.GetAttribute("data-name").Equals(stop))
                    x.Click();
            });

            return IsPostResultsFilterApplied(new List<string> { "Stops" }) && !IsResultsNotAvailableOnFilter();
        }

        private bool SetCabinTypes(List<string> cabinTypes)
        {
            var cabinTypeList = GetUIElements("cabinTypeFilter").ToList();
            cabinTypeList[0].Click();

            cabinTypeList.ForEach(x =>
            {
                if (cabinTypes.Contains(x.GetAttribute("data-name")) && x.Displayed)
                    x.Click();
            });
            return IsPostResultsFilterApplied(new List<string> { "Cabin/Class" }) && !IsResultsNotAvailableOnFilter();
        }

        private bool SetAirlines(List<string> airlines)
        {
            var airlinesList = GetUIElements("airlinesFilter").ToList();
            airlinesList[0].Click();
            airlinesList.ForEach(x =>
            {
                if (airlines.Contains(x.GetAttribute("data-code").ToUpper()) && x.Displayed)
                    x.Click();
            });

            return IsPostResultsFilterApplied(new List<string> { "Airlines" }) && !IsResultsNotAvailableOnFilter();
        }

        private bool SetTakeOffTime(TakeOffTimeRange takeOffTimeRange)
        {
            var jsTslider = WaitAndGetBySelector("jsTslider", ApplicationSettings.TimeOut.Fast);
            if (jsTslider != null && jsTslider.Displayed)
                ExecuteJavascript("var maxTime=parseInt($('.jsTslider').data('slider').max);" +
                                  "var minTime =maxTime- maxTime * " + takeOffTimeRange.Max +
                                  " / 100;maxTime -= maxTime * " + takeOffTimeRange.Min + " / 100;" +
                                  "$('.jsTslider').trigger({type:'slide',value:[minTime,maxTime]}).trigger({type:'slideStop',value:[minTime,maxTime]})");
            return IsPostResultsFilterApplied(new List<string> { "Times" }); // && !IsResultsNotAvailableOnFilter();
        }

        private bool SetLandingTime(LandingTimeRange landingTimeRange)
        {
            var jsLslider = WaitAndGetBySelector("jsLslider", ApplicationSettings.TimeOut.Fast);
            if (jsLslider != null && jsLslider.Displayed)
                ExecuteJavascript("var maxTime=parseInt($('.jsLslider').data('slider').max);" +
                                      "var minTime =maxTime- maxTime * " + landingTimeRange.Max + " / 100;maxTime -= maxTime * " + landingTimeRange.Min + " / 100;" +
                                  "$('.jsLslider').trigger({type:'slide',value:[minTime,maxTime]}).trigger({type:'slideStop',value:[minTime,maxTime]})");

            return IsPostResultsFilterApplied(new List<string> { "Times" });// && !IsResultsNotAvailableOnFilter();
        }

        private float GetFirstItineraryPrice(string sortBy)
        {
            ExecuteJavascript(sortBy.ToLower().Equals("asc") ? "$('#Price-Asc').click()" : "$('#Price-Desc').click()");
            Thread.Sleep(3000);
            return float.Parse(GetUIElements("amount").Select(x => x.Text.Split()[0].TrimStart('$')).ToArray()[0]);
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

        private bool IsResultsNotAvailableOnFilter()
        {
            var resultDiv = WaitAndGetBySelector("zeroAiritineraryDiv", ApplicationSettings.TimeOut.Slow);
            return resultDiv != null && resultDiv.Displayed;
        }

        private bool IsPostResultsFilterApplied(IEnumerable<string> filterType)
        {
            var isFiltered = GetUIElements("appliedFilters").ToList();
            return isFiltered.Exists(x => filterType.Contains(x.GetAttribute("data-fid")));
        }

        private bool SetMatrix()
        {
            var divMatrixAirlines = GetUIElements("divMatrixAirlines");

            divMatrixAirlines[0].Click();

            return !IsResultsNotAvailableOnFilter() &&
                            IsPostResultsFilterApplied(new List<string> { "Airlines", "Price" });

            // uncomment below if wants to test all matrix
            //return !divMatrixAirlines.Any(x =>
            //{
            //    x.Click();
            //    return IsResultsNotAvailableOnFilter() &&
            //           IsPostResultsFilterApplied(new List<string> { "Airlines", "Price" });
            //});
        }

        private void ResetFilters()
        {
            ExecuteJavascript("$('.jsResetAll').click()");
        }

        #endregion

        #region private Air Specific members

        #region need this methods in validations

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

        private bool VerifyDurationFilter(float maxSliderDuration)
        {
            return maxSliderDuration >= GetMaxDurationFlight();
        }

        private bool VerifyPriceFilter(float minSliderPrice, float maxSliderPrice)
        {
            return minSliderPrice <= GetFirstItineraryPrice("asc") && maxSliderPrice >= GetFirstItineraryPrice("desc");
        }

        #endregion

        #endregion

        #region IResultPage Members

        public bool VerifyPreSearchFilters(PreSearchFilters preSearchFilters)
        {
            return true;
        }

        public bool SetPostSearchFilters(PostSearchFilters postSearchFilters)
        {
            var airPostSearchFilters = postSearchFilters as AirPostSearchFilters;
            if (airPostSearchFilters == null || !airPostSearchFilters.IsApplyFilter)
                return true;   
            var stop = airPostSearchFilters.Stop == null || SetStops(airPostSearchFilters.Stop);
            ResetFilters();
            var price = airPostSearchFilters.PriceRange == null || SetPriceRange(airPostSearchFilters.PriceRange);
            ResetFilters();
            var timeDuration = airPostSearchFilters.MaxTimeDurationDiff > 0 ||
                                SetTimeDuration(airPostSearchFilters.MaxTimeDurationDiff);
            ResetFilters();
            var takeOff = airPostSearchFilters.TakeOffTimeRange == null ||
                            SetTakeOffTime(airPostSearchFilters.TakeOffTimeRange);
            ResetFilters();
            var landing = airPostSearchFilters.LandingTimeRange == null ||
                            SetLandingTime(airPostSearchFilters.LandingTimeRange);
            ResetFilters();
            var cabin = airPostSearchFilters.CabinTypes == null || SetCabinTypes(airPostSearchFilters.CabinTypes);
            ResetFilters();
            var airline = airPostSearchFilters.Airlines == null || SetAirlines(airPostSearchFilters.Airlines);
            ResetFilters();
            var matrix = SetMatrix();
            ResetFilters();
            return stop && price && timeDuration && takeOff && landing && cabin && airline && matrix;
        }

        #endregion

    }
}
