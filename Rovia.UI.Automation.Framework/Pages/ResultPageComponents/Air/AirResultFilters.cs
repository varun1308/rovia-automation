namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using AppacitiveAutomationFramework;
    using Exceptions;
    using Logger;
    using ScenarioObjects;
    using Configuration;
    using Utility;

    /// <summary>
    /// Air results page filters container
    /// </summary>
    public class AirResultFilters : UIPage, IResultFilters
    {

        #region Private Fields

        private AirMatrix _airMatrix;
        private List<string> _failedFilters;
        private List<string> _appliedFilters;

        #endregion

        #region Private Members

        private IEnumerable<string> GetAppliedFilters()
        {
            return GetUIElements("appliedFilters").Select(x => x.Text.Trim());
        }
        private void  SetPriceRange(PriceRange priceRange)
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

        private void SetStops(ICollection<string> stopList)
        {
            var stops = GetUIElements("stopsFilter").ToList();
            stops.ForEach(x => x.Click());

            stops.ForEach(x =>
            {
                if (!stopList.Contains(x.GetAttribute("data-name"))) return;
                    x.Click();
                Thread.Sleep(500);
            });
            _appliedFilters.Add("Stops");
        }

        private void SetCabinTypes(List<CabinType> cabinTypes)
        {
            var cabinTypeList = GetUIElements("cabinTypeFilter").ToList();
            cabinTypeList[0].Click();

            cabinTypeList.ForEach(x =>
            {
                if (!cabinTypes.ConvertAll(y=>y.ToString().ToLower()).Contains( x.GetAttribute("data-name")) || !x.Displayed) return;
                    x.Click(); 
                Thread.Sleep(500);
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
                if (!airlines.Contains(x.Text.Trim().ToUpper()) || !x.Displayed) return;
                    x.Click();
                Thread.Sleep(500);
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
            var resultDiv = WaitAndGetBySelector("zeroAiritineraryDiv", ApplicationSettings.TimeOut.Fast);
            if (resultDiv != null && resultDiv.Displayed)
            {
                LogManager.GetInstance().LogWarning(resultDiv.Text);
                throw new UIElementNullOrNotVisible("No reults available for filters validation");
            }
        }

        private void ValidateTripDuration(int maxTimeDuration)
        {
            var legDurations = GetUIElements("legDuration").Select(x => x.Text.Split(':')[1].ToTimeSpan()).ToList();
            var resCount=GetUIElements("subTitle").Count;
            var legsPerResult = legDurations.Count/resCount;
            var resDurations = new List<double>();
            for (var i = 0; i < resCount;i++ )
                resDurations.Add(legDurations.Skip(i * legsPerResult).Take(i * legsPerResult).Sum(x=>x.TotalMinutes));
            if (resDurations.Any(x => x > maxTimeDuration * 60))
                 _failedFilters.Add("Trip Duration");
        }

        private void ValidateCabinTypes(IEnumerable<CabinType> cabinTypes)
        {
            var cabins = GetUIElements("flightInfo").Where((x, i) => i % 2 == 0).Select(x => x.Text.Trim().ToCabinType());
            if (cabins.Any(x => cabinTypes.Min() < x))
                _failedFilters.Add("Cabin/Class");
        }
        
        private void ValidateAirlines(IEnumerable<string> airlines)
        {
            if (!GetAirLines().All(airlines.Contains))
                _failedFilters.Add("Airlines");
        }

        private IEnumerable<string> GetAirLines()
        {
            var subTitles = GetUIElements("subTitle").SelectMany(x => x.Text.ToUpper().Trim().Split('/'));
            var airLines = GetUIElements("title").Select(x => x.Text.ToUpper()).ToList();
            airLines.RemoveAll(x => x.Trim().Equals("Multiple Airlines"));
            airLines.AddRange(subTitles);
            return airLines;
        }

        private void ValidateStops(List<string> filterStopStrings)
        {
            var stops = GetUIElements("flightInfo").Where((x, i) => i%2 == 1).Select(x => int.Parse(x.Text.Trim()));
            var filterStops = GetStops(filterStopStrings);
            if(!stops.All(filterStops.Contains))
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

        private void ValidatePriceRange(PriceRange priceRange)
        {
            if (GetUIElements("amount").Where((x,i)=>i%2==1).Select(x => (new Amount(x.Text)).TotalAmount).Any(x => x > priceRange.MaxPrice || x < priceRange.MinPrice))
                _failedFilters.Add("Price");
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Verify pre search filters on resulting itineraries
        /// </summary>
        /// <param name="preSearchFilters">Applied pre search filter object</param>
        public void VerifyPreSearchFilters(PreSearchFilters preSearchFilters)
        {
            _appliedFilters = GetAppliedFilters().ToList();
            var airFilters = preSearchFilters as AirPreSearchFilters;
            if (preSearchFilters == null)
                throw new InvalidInputException("PreSearchFilters");
            _failedFilters = new List<string>();

            //if (airFilters.AirLines != null && airFilters.AirLines.Count > 0 )
            //    if(!_appliedFilters.Contains("Airlines"))
            //        _failedFilters.Add("Airlines");
            //    else
            //        ValidateAirlines(airFilters.AirLines,airResults.SelectMany(x=>x.AirLines).ToList());
            if (airFilters.CabinType!=CabinType.Economy)
            {
                if (_appliedFilters.Contains("Cabin/Class"))

                    _failedFilters.Add("Cabin/Class");
                else
                ValidateCabinTypes(new List<CabinType>() {airFilters.CabinType});
            }
            if (airFilters.NonStopFlight)
                if (_appliedFilters.Contains("Stops"))
                    ValidateStops(new List<string>(){"none"});
                else
                    _failedFilters.Add("Stops");
            if (_failedFilters.Any())
                throw new ValidationException("Validation Failed for following filters : " + string.Join(",", _failedFilters));
        }

        /// <summary>
        /// Set Filters and Matrix on result page
        /// </summary>
        /// <param name="postSearchFilters">Filters and matrix parameter to set</param>
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
        
        /// <summary>
        /// Validate applied filters and matrix on results page
        /// </summary>
        /// <param name="postSearchFilters">Applied filters and matrix object</param>
        public void ValidateFilters(PostSearchFilters postSearchFilters)
        {
            NoResultsAvailableWarning();
            var airPostSearchFilters = postSearchFilters as AirPostSearchFilters;
            _failedFilters = new List<string>();
            if (airPostSearchFilters == null)
                throw new InvalidInputException("PostSearchFilters");
            if (airPostSearchFilters.PriceRange != null)
                ValidatePriceRange(airPostSearchFilters.PriceRange);
            if (airPostSearchFilters.Stop != null)
                ValidateStops(airPostSearchFilters.Stop);
            if (airPostSearchFilters.Airlines != null)
                ValidateAirlines(airPostSearchFilters.Airlines);
            if (airPostSearchFilters.CabinTypes != null)
                ValidateCabinTypes(airPostSearchFilters.CabinTypes);
            if (airPostSearchFilters.MaxTimeDurationDiff > 0)
                ValidateTripDuration(airPostSearchFilters.MaxTimeDurationDiff);
            if (_failedFilters.Any())
                throw new ValidationException("Validation Failed for following filters : " + string.Join(",", _failedFilters));
        }

        #endregion

    }
}
