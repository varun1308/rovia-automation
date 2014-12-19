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

        public bool VerifyPreSearchFilters(PreSearchFilters preSearchFilters)
        {
            return true;
        }

        public void SetPostSearchFilters(PostSearchFilters postSearchFilters)
        {
            var hotelPostSearchFilters = postSearchFilters as HotelPostSearchFilters;
            if (hotelPostSearchFilters == null)
                throw new InvalidInputException("PostSearchFilters");
            var appliedFilters = new List<string>();
            if (hotelPostSearchFilters.PriceRange != null)
            {
                SetPriceRange(hotelPostSearchFilters.PriceRange);
                appliedFilters.Add("Price");
            }
            if (hotelPostSearchFilters.HotelName != null)
            {
                SetHotelName(hotelPostSearchFilters.HotelName);
                appliedFilters.Add("Hotel Name");
            }
            if (hotelPostSearchFilters.RatingRange != null)
            {
                SetRatingRange(hotelPostSearchFilters.RatingRange);
                appliedFilters.Add("Star Rating");
            }
            if (hotelPostSearchFilters.Amenities != null)
            {
                SetAmenitiesFilter(hotelPostSearchFilters.Amenities);
                appliedFilters.Add("Amenities");
            }
            if (hotelPostSearchFilters.PreferredLocation != null)
            {
                SetPreferredLocationFilter(hotelPostSearchFilters.PreferredLocation);
                appliedFilters.Add("Preferred Location");
            }
            if (hotelPostSearchFilters.DistanceRange != null)
            {
                SetDistanceRangeFilter(hotelPostSearchFilters.DistanceRange);
                appliedFilters.Add("Distance");
            }

            var unAppliedFilters = appliedFilters.Except(GetAppliedFilters()).ToList();

            if (unAppliedFilters.Any())
                throw new ValidationException("Following Filters were not applied : " + string.Join(",", unAppliedFilters));
        }

        public void ValidateFilters(PostSearchFilters postSearchFilters, List<Results> results)
        {
            var hotelPostSearchFilters = postSearchFilters as HotelPostSearchFilters;
            var hotelResults = results.Select(x => x as HotelResult).ToList();
            if (hotelPostSearchFilters == null)
                throw new InvalidInputException("PostSearchFilters");
            var appliedFilters = new List<string>();
            if (hotelPostSearchFilters.PriceRange != null)
            {
                ValidatePriceRange(hotelResults.Select(x=>x.Amount));
                appliedFilters.Add("Price");
            }
            if (hotelPostSearchFilters.HotelName != null)
            {
                ValidateHotelName(hotelPostSearchFilters.HotelName, hotelResults.Select(x=>x.HotelName));
                appliedFilters.Add("Hotel Name");
            }
            if (hotelPostSearchFilters.RatingRange != null)
            {
                ValidateRatingRange(hotelPostSearchFilters.RatingRange, hotelResults.Select(x=>x.HotelRating));
                appliedFilters.Add("Star Rating");
            }
            if (hotelPostSearchFilters.Amenities != null)
            {
                ValidateAmenitiesFilter(hotelPostSearchFilters.Amenities, hotelResults.Select(x=>x.Amenities));
                appliedFilters.Add("Amenities");
            }
            if (hotelPostSearchFilters.PreferredLocation != null)
            {
                ValidatePreferredLocationFilter(hotelPostSearchFilters.PreferredLocation, hotelResults.Select(x=>x.HotelAddress));
                appliedFilters.Add("Preferred Location");
            }
            if (hotelPostSearchFilters.DistanceRange != null)
            {
                SetDistanceRangeFilter(hotelPostSearchFilters.DistanceRange);
                appliedFilters.Add("Distance");
            }

        }

        private void ValidatePreferredLocationFilter(Tuple<string, string> preferredLocation, IEnumerable<string> @select)
        {
            throw new NotImplementedException();
        }

        private void ValidateAmenitiesFilter(List<string> amenities, IEnumerable<List<string>> @select)
        {
            throw new NotImplementedException();
        }

        private void ValidateRatingRange(RatingRange ratingRange, IEnumerable<int> @select)
        {
            throw new NotImplementedException();
        }

        private void ValidateHotelName(string hotelName, IEnumerable<string> @select)
        {
            throw new NotImplementedException();
        }

        private void ValidatePriceRange(IEnumerable<Amount> enumerable)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region private Hotel Specific members
        private IEnumerable<string> GetAppliedFilters()
        {
            Thread.Sleep(2000);
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
