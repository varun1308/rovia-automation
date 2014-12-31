using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public class AirResultFilters : UIPage, IResultFilters
    {
        #region private Air Specific members

        private AirMatrix _airMatrix;
        private List<string> _failedFilters;

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
        }

        private void SetTimeDuration(int maxTimeDurationCustom)
        {
            //taken out the max duration from slider
            var maxTimeDurationMins =
                WaitAndGetBySelector("maxTimeDuration", ApplicationSettings.TimeOut.Fast).Text.Split(' ');
            ExecuteJavascript("$('#sliderTripDuration').trigger({type:'slideStop',value:[" + (maxTimeDurationCustom * 60) + "]})");
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
        }

        private void SetAirlines(List<string> airlines)
        {
            var airlinesList = GetUIElements("airlinesFilter").ToList();
            airlinesList[0].Click();
            airlinesList.ForEach(x =>
            {
                if (airlines.Contains(x.GetAttribute("data-code").ToUpper()) && x.Displayed)
                    x.Click();
            });
        }

        private void SetTakeOffTime(TakeOffTimeRange takeOffTimeRange)
        {
            var jsTslider = WaitAndGetBySelector("jsTslider", ApplicationSettings.TimeOut.Fast);
            if (jsTslider != null && jsTslider.Displayed)
                ExecuteJavascript("var maxTime=parseInt($('.jsTslider').data('slider').max);" +
                                  "var minTime =maxTime- maxTime * " + takeOffTimeRange.Max +
                                  " / 100;maxTime -= maxTime * " + takeOffTimeRange.Min + " / 100;" +
                                  "$('.jsTslider').trigger({type:'slide',value:[minTime,maxTime]}).trigger({type:'slideStop',value:[minTime,maxTime]})");
        }

        private void SetLandingTime(LandingTimeRange landingTimeRange)
        {
            var jsLslider = WaitAndGetBySelector("jsLslider", ApplicationSettings.TimeOut.Fast);
            if (jsLslider != null && jsLslider.Displayed)
                ExecuteJavascript("var maxTime=parseInt($('.jsLslider').data('slider').max);" +
                                      "var minTime =maxTime- maxTime * " + landingTimeRange.Max + " / 100;maxTime -= maxTime * " + landingTimeRange.Min + " / 100;" +
                                  "$('.jsLslider').trigger({type:'slide',value:[minTime,maxTime]}).trigger({type:'slideStop',value:[minTime,maxTime]})");
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

            // uncomment below if wants to test all matrix
            //return !divMatrixAirlines.Any(x =>
            //{
            //    x.Click();
            //    return IsResultsNotAvailableOnFilter() &&
            //           IsPostResultsFilterApplied(new List<string> { "Airlines", "Price" });
            //});
        }


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

        public bool VerifyPreSearchFilters(PreSearchFilters preSearchFilters, Func<List<Results>> getParsedResults)
        {
            return true;
        }

        public void SetPostSearchFilters(PostSearchFilters postSearchFilters)
        {
            var airPostSearchFilters = postSearchFilters as AirPostSearchFilters;
            if (airPostSearchFilters == null)
                throw new InvalidInputException("PostSearchFilters");
            var appliedFilters = new List<string>();
            if (airPostSearchFilters.Stop != null)
            {
                SetStops(airPostSearchFilters.Stop);
                appliedFilters.Add("Stops");
            }
            if (airPostSearchFilters.PriceRange != null)
            {
                SetPriceRange(airPostSearchFilters.PriceRange);
                appliedFilters.Add("Price");
            }
            if (airPostSearchFilters.MaxTimeDurationDiff >= 0)
            {
                SetTimeDuration(airPostSearchFilters.MaxTimeDurationDiff);
                appliedFilters.Add("Trip Duration");
            }
            if (airPostSearchFilters.TakeOffTimeRange != null)
            {
                SetTakeOffTime(airPostSearchFilters.TakeOffTimeRange);
                appliedFilters.Add("Times");
            }
            if (airPostSearchFilters.LandingTimeRange != null)
            {
                SetLandingTime(airPostSearchFilters.LandingTimeRange);
                appliedFilters.Add("Times");
            }
            if (airPostSearchFilters.CabinTypes != null)
            {
                SetCabinTypes(airPostSearchFilters.CabinTypes);
                appliedFilters.Add("Cabin/Class");
            }
            if (airPostSearchFilters.Airlines != null)
            {
                SetAirlines(airPostSearchFilters.Airlines);
                appliedFilters.Add("Airlines");
            }
            if (airPostSearchFilters.Matrix != null)
            {
                SetMatrix();
                appliedFilters.AddRange(new string[] { "Airlines", "Price" });
            }
            var unAppliedFilters = appliedFilters.Except(GetAppliedFilters()).ToList();

            if (unAppliedFilters.Any())
                throw new ValidationException("Following Filters were not applied : " + string.Join(",", unAppliedFilters));
        }

        public void ValidateFilters(PostSearchFilters postSearchFilters, Func<List<Results>> getParsedResults)
        {
            var airPostSearchFilters = postSearchFilters as AirPostSearchFilters;
            var airResults = getParsedResults().Select(x => x as AirResult).ToList();
            _failedFilters = new List<string>();
            if (airPostSearchFilters == null)
                throw new InvalidInputException("PostSearchFilters");
            if (airPostSearchFilters.PriceRange != null)
                ValidatePriceRange(airPostSearchFilters.PriceRange, airResults.Select(x => x.Amount.TotalAmount));
            //if (airPostSearchFilters.Stop != null)
            //{
            //    ValidateStops(airPostSearchFilters.Stop.ToList().ConvertAll(x=>x.ToLower()), airResults.Select(x => x.Legs));
            //}
            if (_failedFilters.Any())
                throw new ValidationException("Validation Failed for following filters : " + string.Join(",", _failedFilters));
        }

        private void ValidateStops(List<string> filterStops, IEnumerable<List<FlightLegs>> resultLegs)
        {

        }

        private List<int> GetStops(string[] filterStops)
        {
            var i = 0;
            var stopLists = new List<int>();
            while (i < filterStops.Length)
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

    }
}
