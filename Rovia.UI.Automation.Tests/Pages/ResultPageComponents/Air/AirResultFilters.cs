using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public class AirResultFilters : UIPage, IResultFilters
    {
        #region private Air Specific members

        private AirMatrix _airMatrix;
        private List<string> _failedFilters;
        private List<string> _appliedFilters;

        private IEnumerable<string> GetAppliedFilters()
        {
            return GetUIElements("appliedFilters").Select(x => x.Text.Trim());
        }
        private void SetPriceRange(PriceRange priceRange)
        {
            var minPrice =
                float.Parse(WaitAndGetBySelector("minPrice", ApplicationSettings.TimeOut.Fast).Text.Split(' ')[0].TrimStart('$'));
            var maxPrice =
               float.Parse(WaitAndGetBySelector("maxPrice", ApplicationSettings.TimeOut.Fast).Text.Split(' ')[0].TrimStart('$'));
            priceRange.MinPrice = minPrice + (minPrice * priceRange.Min / 100);
            priceRange.MaxPrice = maxPrice - (maxPrice * priceRange.Max / 100);
            ExecuteJavascript("$('#sliderRangePrice').trigger({type:'slideStop',value:[" + (priceRange.MinPrice * 100) + "," + (priceRange.MaxPrice * 100) + "]})");
            _appliedFilters.Add("Price");
        }

        private void SetTimeDuration(int maxTimeDurationCustom)
        {
            //taken out the max duration from slider
            var maxTimeDurationMins =
                WaitAndGetBySelector("maxTimeDuration", ApplicationSettings.TimeOut.Fast).Text.Split(' ');
            ExecuteJavascript("$('#sliderTripDuration').trigger({type:'slideStop',value:[" + (maxTimeDurationCustom * 60) + "]})");
            _appliedFilters.Add("Trip Duration");
        }

        private void SetStops(List<string> stopList)
        {
            var stops = GetUIElements("stopsFilter").ToList();
            stops.ForEach(x => x.Click());

            stops.ForEach(x =>
            {
                if (stopList.Contains(x.GetAttribute("data-name")))
                    x.Click();
            });
            _appliedFilters.Add("Stops");
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

            if (!cabinTypeList[0].Selected)
                _appliedFilters.Add("Cabin/Class");
        }

        private void SetAirlines(List<string> airlines)
        {
            var airlinesList = GetUIElements("airlinesFilter").ToList();
            airlinesList[0].Click();
            airlinesList.ForEach(x =>
            {
                if (airlines.Contains(x.Text.Trim().ToUpper()) && x.Displayed)
                    x.Click();
            });
            if (!airlinesList[0].Selected)
                _appliedFilters.Add("Airlines");
        }

        private void SetTakeOffTime(TakeOffTimeRange takeOffTimeRange)
        {
            var jsTslider = WaitAndGetBySelector("jsTslider", ApplicationSettings.TimeOut.Fast);
            if (jsTslider != null && jsTslider.Displayed)
                ExecuteJavascript("var maxTime=parseInt($('.jsTslider').data('slider').max);" +
                                  "var minTime =maxTime- maxTime * " + takeOffTimeRange.Max +
                                  " / 100;maxTime -= maxTime * " + takeOffTimeRange.Min + " / 100;" +
                                  "$('.jsTslider').trigger({type:'slide',value:[minTime,maxTime]}).trigger({type:'slideStop',value:[minTime,maxTime]})");
            _appliedFilters.Add("Times");
        }

        private void SetLandingTime(LandingTimeRange landingTimeRange)
        {
            var jsLslider = WaitAndGetBySelector("jsLslider", ApplicationSettings.TimeOut.Fast);
            if (jsLslider != null && jsLslider.Displayed)
                ExecuteJavascript("var maxTime=parseInt($('.jsLslider').data('slider').max);" +
                                      "var minTime =maxTime- maxTime * " + landingTimeRange.Max + " / 100;maxTime -= maxTime * " + landingTimeRange.Min + " / 100;" +
                                  "$('.jsLslider').trigger({type:'slide',value:[minTime,maxTime]}).trigger({type:'slideStop',value:[minTime,maxTime]})");
            _appliedFilters.Add("Times");
        }

        private void SetMatrix()
        {
            var divMatrixAirlines = GetUIElements("divMatrix").First(x => x.WaitAndGetBySelector("divMatrixPrice", ApplicationSettings.TimeOut.Fast).Text.Contains('$'));
            _airMatrix = new AirMatrix
                {
                    Airlines =
                        divMatrixAirlines.WaitAndGetBySelector("divMatrixAirlines", ApplicationSettings.TimeOut.Fast).
                            GetAttribute("title"),
                    Price =
                        divMatrixAirlines.WaitAndGetBySelector("divMatrixPrice", ApplicationSettings.TimeOut.Fast).Text
                };
            divMatrixAirlines.Click();

            _appliedFilters.AddRange(new string[] { "Airlines", "Price" });

            // uncomment below if wants to test all matrix
            //return !divMatrixAirlines.Any(x =>
            //{
            //    x.Click();
            //    return IsResultsNotAvailableOnFilter() &&
            //           IsPostResultsFilterApplied(new List<string> { "Airlines", "Price" });
            //});
        }

        private void NoResultsAvailableWarning()
        {
            var resultDiv = WaitAndGetBySelector("zeroAiritineraryDiv", ApplicationSettings.TimeOut.Slow);
            if (resultDiv != null && resultDiv.Displayed)
            {
                LogManager.GetInstance().LogWarning(resultDiv.Text);
                throw new UIElementNullOrNotVisible("No reults available for filters validation");
            }
        }

        private void ValidateTripDuration(int maxTimeDuration, IEnumerable<int> resultDuration)
        {
            if (resultDuration.Any(x => x > maxTimeDuration * 60))
                _failedFilters.Add("Trip Duration");
        }

        private void ValidateCabinTypes(IEnumerable<string> cabinTypes, IEnumerable<IEnumerable<CabinType>> resultcabins)
        {
            if (cabinTypes.Any(x => resultcabins.Any(y => !y.Contains(StringToEnum<CabinType>(x)))))
                _failedFilters.Add("Cabin/Class");
        }

        private static T StringToEnum<T>(string name)
        {
            return (T)Enum.Parse(typeof(T), name, true);
        }

        private void ValidateAirlines(IEnumerable<string> airlines, IEnumerable<List<string>> resultAirlines)
        {
            if (resultAirlines.Any(y => !y.ConvertAll(z => z.ToUpper()).TrueForAll(airlines.Contains)))
                _failedFilters.Add("Airlines");
        }

        private void ValidateStops(List<string> filterStops, IEnumerable<int> resultStops)
        {
            if (GetStops(filterStops).Any(x => resultStops.Any(y => y != x)))
                _failedFilters.Add("Stops");
        }

        private IEnumerable<int> GetStops(IList<string> filterStops)
        {
            var i = 0;
            var stopLists = new List<int>();
            while (i < filterStops.Count)
            {
                switch (filterStops[i].ToUpper())
                {
                    case "NONE": stopLists.Add(0);
                        break;
                    case "ONE": stopLists.Add(1);
                        break;
                    case "ONEPLUS": stopLists.Add(2);
                        break;
                    default: stopLists.Add(0);
                        break;
                }
                i++;
            }
            return stopLists;
        }

        private void ValidatePriceRange(PriceRange priceRange, IEnumerable<double> amountList)
        {
            if (amountList.Any(x => x > priceRange.MaxPrice || x < priceRange.MinPrice))
                _failedFilters.Add("Price");
        }

        #endregion

        #region IResultPage Members

        public void VerifyPreSearchFilters(PreSearchFilters preSearchFilters, Func<List<Results>> getParsedResults)
        {
            return;
        }

        public void SetPostSearchFilters(PostSearchFilters postSearchFilters)
        {
            var airPostSearchFilters = postSearchFilters as AirPostSearchFilters;
            if (airPostSearchFilters == null)
                throw new InvalidInputException("PostSearchFilters");
            _appliedFilters = new List<string>();
            if (airPostSearchFilters.Stop != null)
                SetStops(airPostSearchFilters.Stop);
            if (airPostSearchFilters.PriceRange != null)
                SetPriceRange(airPostSearchFilters.PriceRange);
            if (airPostSearchFilters.MaxTimeDurationDiff > 0)
                SetTimeDuration(airPostSearchFilters.MaxTimeDurationDiff);
            if (airPostSearchFilters.TakeOffTimeRange != null)
                SetTakeOffTime(airPostSearchFilters.TakeOffTimeRange);
            if (airPostSearchFilters.LandingTimeRange != null)
                SetLandingTime(airPostSearchFilters.LandingTimeRange);
            if (airPostSearchFilters.CabinTypes != null)
                SetCabinTypes(airPostSearchFilters.CabinTypes);
            if (airPostSearchFilters.Airlines != null)
                SetAirlines(airPostSearchFilters.Airlines);
            if (airPostSearchFilters.Matrix != null)
                SetMatrix();

            var unAppliedFilters = _appliedFilters.Except(GetAppliedFilters()).ToList();

            if (unAppliedFilters.Any())
                throw new ValidationException("Following Filters were not applied : " + string.Join(",", unAppliedFilters));
        }

        public void ValidateFilters(PostSearchFilters postSearchFilters, Func<List<Results>> getParsedResults)
        {
            NoResultsAvailableWarning();
            var airPostSearchFilters = postSearchFilters as AirPostSearchFilters;
            var airResults = getParsedResults().Select(x => x as AirResult).ToList();
            _failedFilters = new List<string>();
            if (airPostSearchFilters == null)
                throw new InvalidInputException("PostSearchFilters");

            if (airPostSearchFilters.PriceRange != null)
                ValidatePriceRange(airPostSearchFilters.PriceRange, airResults.Select(x => x.Amount.TotalAmount));
            if (airPostSearchFilters.Stop != null)
                ValidateStops(airPostSearchFilters.Stop, airResults.Select(x => x.Legs.Select(y => y.Stops)).Select(z => z.Sum()).ToList());
            if (airPostSearchFilters.Airlines != null)
                ValidateAirlines(airPostSearchFilters.Airlines, airResults.Select(x => x.AirLines).ToList());
            if (airPostSearchFilters.CabinTypes != null)
                ValidateCabinTypes(airPostSearchFilters.CabinTypes, airResults.Select(x => x.Legs.Select(y => y.Cabin)).ToList());
            if (airPostSearchFilters.MaxTimeDurationDiff > 0)
                ValidateTripDuration(airPostSearchFilters.MaxTimeDurationDiff, airResults.Select(x => x.Legs.Select(y => y.Duration)).Select(z => z.Sum()).ToList());
            if (_failedFilters.Any())
                throw new ValidationException("Validation Failed for following filters : " + string.Join(",", _failedFilters));
        }

        #endregion

    }
}
