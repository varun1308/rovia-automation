using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.Ui.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    class HotelResultFilters : UIPage, IResultFilters
    {
        #region IResultPage Members
        public void SetPostSearchFilters(PostSearchFilters postSearchFilters)
        {
            var airPostSearchFilters = postSearchFilters as HotelPostSearchFilters;
            if (airPostSearchFilters == null)
                throw new InvalidInputException("PostSearchFilters");
            var appliedFilters = new List<string>();
            if (airPostSearchFilters.PriceRange != null)
            {
                SetPriceRange(airPostSearchFilters.PriceRange);
                appliedFilters.Add("Price");
            }
            if (airPostSearchFilters.HotelName != null)
            {
                SetHotelName(airPostSearchFilters.HotelName);
                appliedFilters.Add("Hotel Name");
            }
            if (airPostSearchFilters.RatingRange != null)
            {
                SetRatingRange(airPostSearchFilters.RatingRange);
                appliedFilters.Add("Star Rating");
            }
            if (airPostSearchFilters.Amenities != null)
            {
                SetAmenitiesFilter(airPostSearchFilters.Amenities);
                appliedFilters.Add("Amenities");
            }
            if (airPostSearchFilters.PreferredLocation != null)
            {
                SetPreferredLocationFilter(airPostSearchFilters.PreferredLocation);
                appliedFilters.Add("Preferred Location");
            }
            if (airPostSearchFilters.DistanceRange != null)
            {
                SetDistanceRangeFilter(airPostSearchFilters.DistanceRange);
                appliedFilters.Add("Distance");
            }

            var unAppliedFilters = appliedFilters.Except(GetAppliedFilters()).ToList();

            if (unAppliedFilters.Any())
                throw new ValidationException("Following Filters were not applied : " + string.Join(",", unAppliedFilters));
        }
        #endregion

        #region private Hotel Specific members
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

            minPrice += minPrice * priceRange.Min / 100;
            maxPrice -= maxPrice * priceRange.Max / 100;

            ExecuteJavascript("$('#sliderRangePrice').trigger({type:'slideStop',value:[" + (minPrice * 100) + "," + (maxPrice * 100) + "]})");
        }

        private void SetDistanceRangeFilter(DistanceRange distanceRange)
        {
            var minDistance =
                double.Parse(WaitAndGetBySelector("minDistance", ApplicationSettings.TimeOut.Fast).Text.Split(' ')[0].TrimStart('$'));
            var maxDistance =
               double.Parse(WaitAndGetBySelector("maxDistance", ApplicationSettings.TimeOut.Fast).Text.Split(' ')[0].TrimStart('$'));

            minDistance += minDistance * distanceRange.Min / 100;
            maxDistance -= maxDistance * distanceRange.Max / 100;

            ExecuteJavascript("$('#sliderRangeDistance').trigger({type:'slideStop',value:[" + (minDistance * 100) + "," + (maxDistance * 100) + "]})");
        }

        private void SetPreferredLocationFilter(Tuple<string, string> preferredLocation)
        {
            WaitAndGetBySelector("categoryHolder", ApplicationSettings.TimeOut.Fast).Click();
            GetUIElements("preferredLocationCategory").First(x => x.GetAttribute("innerText").Equals(preferredLocation.Item1)).Click();
            WaitAndGetBySelector("locationHolder", ApplicationSettings.TimeOut.Fast).Click();
            GetUIElements("preferredLocation").First(x => x.GetAttribute("innerText").Equals(preferredLocation.Item2)).Click();
        }

        private void SetAmenitiesFilter(ICollection<string> amenityList)
        {
            WaitAndGetBySelector("amenitiesResetLink", ApplicationSettings.TimeOut.Fast).Click();
            GetUIElements("amenities").ForEach(x =>
                {
                    if (amenityList.Contains(x.GetAttribute("data-name")))
                        x.Click();
                    Thread.Sleep(500);
                });
        }

        private void SetRatingRange(RatingRange ratingRange)
        {
            WaitAndGetBySelector("starRatingResetLink", ApplicationSettings.TimeOut.Fast).Click();
            GetUIElements("minStarRating")[ratingRange.From - 1].Click();
            if (ratingRange.From < 5)
                GetUIElements("maxStarRating")[ratingRange.To].Click();
        }

        private void SetHotelName(string hotelName)
        {
            WaitAndGetBySelector("textHotelName", ApplicationSettings.TimeOut.Fast).SendKeys(hotelName);
            WaitAndGetBySelector("searchIcon", ApplicationSettings.TimeOut.Fast).Click();
        }
        #endregion
    }
}
