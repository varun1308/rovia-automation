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
    class HotelResultFilters : UIPage, IResultFilters
    {
        private List<string> _appliedFilters;
        private List<string> _failedFilters;

        #region IResultFilter Members

        public void VerifyPreSearchFilters(PreSearchFilters preSearchFilters)
        {
            if (!AreResultsAvailable())
                return;
            _appliedFilters = GetAppliedFilters().ToList();
            var hotelFilters = preSearchFilters as HotelPreSearchFilters;
            if (hotelFilters == null)
                throw new InvalidInputException("PreSearchFilters");
            _failedFilters = new List<string>();

            if (hotelFilters.AdditionalPreferences != null && hotelFilters.AdditionalPreferences.Count > 0 && !_appliedFilters.Contains("Amenities"))
                _failedFilters.Add("Amenities");
            if (!string.IsNullOrEmpty(hotelFilters.HotelName))
            {
                if (_appliedFilters.Contains("Hotel Name"))
                    ValidateHotelNameFilter(hotelFilters.HotelName);
                else
                    _failedFilters.Add("Hotel Name");
            }
            if (!string.IsNullOrEmpty(hotelFilters.StarRating))
                if (_appliedFilters.Contains("Star Rating"))
                    ValidateRatingRangeFilter(new RatingRange() { From = int.Parse(hotelFilters.StarRating), To = 5 });
                else
                    _failedFilters.Add("Star Rating");
            if (_failedFilters.Any())
                throw new ValidationException("Validation Failed for following filters : " + string.Join(",", _failedFilters));
        }

        public void SetPostSearchFilters(PostSearchFilters postSearchFilters)
        {
            var hotelPostSearchFilters = postSearchFilters as HotelPostSearchFilters;
            if (hotelPostSearchFilters == null)
                throw new InvalidInputException("PostSearchFilters");
            _appliedFilters = new List<string>();
            if (hotelPostSearchFilters.Matrix != null)
            {
                SetMatrix(hotelPostSearchFilters.Matrix as HotelMatrix);
                if (!AreResultsAvailable())
                    hotelPostSearchFilters.Matrix = null;
            }
            if (hotelPostSearchFilters.PriceRange != null)
            {
                SetPriceRangeFilter(hotelPostSearchFilters.PriceRange);
                if (!AreResultsAvailable())
                    hotelPostSearchFilters.PriceRange = null;
                else
                    _appliedFilters.Add("Price");
            }
            if (hotelPostSearchFilters.HotelName != null)
            {
                SetHotelNameFilter(hotelPostSearchFilters.HotelName);
                if (!AreResultsAvailable())
                    hotelPostSearchFilters.HotelName = null;
                else
                    _appliedFilters.Add("Hotel Name");
            }
            if (hotelPostSearchFilters.RatingRange != null)
            {
                SetRatingRangeFilter(hotelPostSearchFilters.RatingRange);
                if (!AreResultsAvailable())
                    hotelPostSearchFilters.RatingRange = null;
                else
                    _appliedFilters.Add("Star Rating");
            }
            if (hotelPostSearchFilters.Amenities != null)
            {
                SetAmenitiesFilter(hotelPostSearchFilters.Amenities);
                if (!AreResultsAvailable())
                    hotelPostSearchFilters.Amenities = null;
                else
                    _appliedFilters.Add("Amenities");
            }
            if (hotelPostSearchFilters.PreferredLocation != null)
            {
                SetPreferredLocationFilter(hotelPostSearchFilters.PreferredLocation);
                if (!AreResultsAvailable())
                    hotelPostSearchFilters.PreferredLocation = null;
                else
                    _appliedFilters.Add("Preferred Location");
            }
            if (hotelPostSearchFilters.DistanceRange != null)
            {
                SetDistanceRangeFilter(hotelPostSearchFilters.DistanceRange);
                if (!AreResultsAvailable())
                    hotelPostSearchFilters.DistanceRange = null;
                else
                    _appliedFilters.Add("Distance");
            }
            if (hotelPostSearchFilters.SortBy != SortBy.Featured)
            {
                SortResults(hotelPostSearchFilters.SortBy);
            }
            var unAppliedFilters = _appliedFilters.Except(GetAppliedFilters()).ToList();
            if (unAppliedFilters.Any())
                throw new ValidationException("Following Filters were not applied : " + string.Join(",", unAppliedFilters));
        }

        public void ValidateFilters(PostSearchFilters postSearchFilters)
        {
            var hotelPostSearchFilters = postSearchFilters as HotelPostSearchFilters;
            _failedFilters = new List<string>();
            if (hotelPostSearchFilters == null)
                throw new InvalidInputException("PostSearchFilters");
            if (hotelPostSearchFilters.PriceRange != null)
                ValidatePriceRangeFilter(hotelPostSearchFilters.PriceRange);
            if (hotelPostSearchFilters.HotelName != null)
                ValidateHotelNameFilter(hotelPostSearchFilters.HotelName);
            if (hotelPostSearchFilters.RatingRange != null)
                ValidateRatingRangeFilter(hotelPostSearchFilters.RatingRange);
            if (hotelPostSearchFilters.SortBy != SortBy.Featured)
                ValidateSort(hotelPostSearchFilters.SortBy);
            if (hotelPostSearchFilters.Matrix != null)
            {
                ValidateMatrix(hotelPostSearchFilters.Matrix as HotelMatrix);
                SortResults(hotelPostSearchFilters.SortBy);
            }
            if (_failedFilters.Any())
                throw new ValidationException("Validation Failed for following filters : " + string.Join(",", _failedFilters));
        }

        #endregion

        #region private Hotel Specific members
        
        private void WaitWhilePreLoaderIsDisplayed()
        {
            var preloader = WaitAndGetBySelector("preLoader", ApplicationSettings.TimeOut.Fast);
            while (preloader.Displayed) ;
        }

        private bool AreResultsAvailable()
        {
            foreach (var error in GetUIElements("error").Where(error => error.Displayed))
            {
                LogManager.GetInstance().LogWarning(error.Text);
                return false;
            }
            return true;
        }

        private IEnumerable<string> GetAppliedFilters()
        {
            Thread.Sleep(2000);
            return GetUIElements("appliedFilters").Select(x => x.Text.Trim());
        }

        private void ValidateMatrix(HotelMatrix matrix)
        {
            SortResults(SortBy.PriceAsc);
            var ratings = GetUIElements("hotelRating").Select(x => x.GetUIElements("stars").Count);
            var prices = GetUIElements("hotelPrice").Select(x => new Amount(x.Text).TotalAmount);
            if (!prices.First().Equals(matrix.Amount.TotalAmount) || ratings.Any(x => (x != matrix.Rating)))
                _failedFilters.Add("Matrix");
        }

        private void ValidateSort(SortBy sortBy)
        {
            switch (sortBy)
            {
                case SortBy.PriceAsc: ValidatePriceSort(sortBy);
                    break;
                case SortBy.PriceDsc: ValidatePriceSort(sortBy);
                    break;
                case SortBy.NameAsc: ValidateNameSort(sortBy);
                    break;
                case SortBy.NameDsc: ValidateNameSort(sortBy);
                    break;
                case SortBy.RatingAsc: ValidateRatingSort(sortBy);
                    break;
                case SortBy.RatingDsc: ValidateRatingSort(sortBy);
                    break;
            }
            WaitWhilePreLoaderIsDisplayed();
        }

        private void ValidateRatingSort(SortBy order)
        {
            var ratings = GetUIElements("hotelRating").Select(x => x.GetUIElements("stars").Count).ToArray();
            if(order==SortBy.RatingDsc)
                Array.Reverse(ratings);
            for (var i = 1; i < ratings.Length; i++)
            {
                if (ratings[i] >= ratings[i - 1]) continue;
                _failedFilters.Add("RatingSort");
                return;
            }
        }

        private void ValidateNameSort(SortBy order)
        {
            var hotelNames = GetUIElements("divHotelName").Select(x => x.Text).ToArray();
            if(order==SortBy.NameDsc)
                Array.Reverse(hotelNames);
            for (var i = 1; i < hotelNames.Length; i++)
            {
                if (String.CompareOrdinal(hotelNames[i].ToUpper(), hotelNames[i - 1].ToUpper()) >= 0) continue;
                _failedFilters.Add("HotelNameSort");
                return;
            }
        }

        private void ValidatePriceSort(SortBy order)
        {
            var prices = GetUIElements("hotelPrice").Select(x => new Amount(x.Text).TotalAmount).ToArray();
            if(order==SortBy.PriceDsc)
                Array.Reverse(prices);
            for (var i = 1; i < prices.Length; i++)
            {
                if (prices[i] >= prices[i - 1]) continue;
                _failedFilters.Add("PriceSort");
                return;
            }
        }

        private void ValidateRatingRangeFilter(RatingRange ratingRange)
        {
            var ratings = GetUIElements("hotelRating").Select(x => x.GetUIElements("stars").Count);
            if (ratings.Any(x => x > ratingRange.To || x < ratingRange.From))
                _failedFilters.Add("Rating");
        }

        private void ValidateHotelNameFilter(string hotelName)
        {
            var hotelNames = GetUIElements("divHotelName").Select(x => x.Text);
            if (hotelNames.Any(x => !x.Contains(hotelName)))
                _failedFilters.Add("Hotel Name");
        }

        private void ValidatePriceRangeFilter(PriceRange priceRange)
        {
            var prices = GetUIElements("hotelPrice").Select(x => new Amount(x.Text).TotalAmount);
            if (prices.Any(x => x > priceRange.MaxPrice || x < priceRange.MinPrice))
                _failedFilters.Add("Price");
        }
        
        private void SetMatrix(HotelMatrix matrix)
        {
            try
            {
                var matrixSection = GetUIElements("matrixSection").First(x => x.GetUIElements("ratedStars").Count == matrix.Rating);
                matrixSection.Click();
                WaitWhilePreLoaderIsDisplayed();
                matrix.Amount = new Amount(matrixSection.WaitAndGetBySelector("matrixPrice", ApplicationSettings.TimeOut.Fast).Text);
                _appliedFilters.AddRange(new[] { "Star Rating", "Price" });
            }
            catch (System.InvalidOperationException)
            {
                LogManager.GetInstance().LogWarning(string.Format("Matrix Filter was not applied( Requested Matrix Section with Rating : {0} not Found )", matrix.Rating));
            }
        }

        private void SortResults(SortBy sortBy)
        {
            WaitAndGetBySelector("sortHeader", ApplicationSettings.TimeOut.Safe).Click();
            switch (sortBy)
            {
                case SortBy.Featured: WaitAndGetBySelector("featured", ApplicationSettings.TimeOut.Fast).Click();
                    break;
                case SortBy.PriceAsc: WaitAndGetBySelector("priceAsc", ApplicationSettings.TimeOut.Fast).Click();
                    break;
                case SortBy.PriceDsc: WaitAndGetBySelector("priceDsc", ApplicationSettings.TimeOut.Fast).Click();
                    break;
                case SortBy.NameAsc: WaitAndGetBySelector("nameAsc", ApplicationSettings.TimeOut.Fast).Click();
                    break;
                case SortBy.NameDsc: WaitAndGetBySelector("nameDsc", ApplicationSettings.TimeOut.Fast).Click();
                    break;
                case SortBy.RatingAsc: WaitAndGetBySelector("ratingAsc", ApplicationSettings.TimeOut.Fast).Click();
                    break;
                case SortBy.RatingDsc: WaitAndGetBySelector("ratingDsc", ApplicationSettings.TimeOut.Fast).Click();
                    break;
            }
            WaitWhilePreLoaderIsDisplayed();
        }
        
        private void SetPriceRangeFilter(PriceRange priceRange)
        {
            var minPrice =
                float.Parse(WaitAndGetBySelector("minPrice", ApplicationSettings.TimeOut.Fast).Text.Split(' ')[0].TrimStart('$'));
            var maxPrice =
               float.Parse(WaitAndGetBySelector("maxPrice", ApplicationSettings.TimeOut.Fast).Text.Split(' ')[0].TrimStart('$'));

            priceRange.MinPrice = minPrice + (minPrice * priceRange.Min / 100);
            priceRange.MaxPrice = maxPrice - (maxPrice * priceRange.Max / 100);
            ExecuteJavascript("$('#sliderRangePrice').trigger({type:'slideStop',value:[" + (priceRange.MinPrice * 100) + "," + (priceRange.MaxPrice * 100) + "]})");
            WaitWhilePreLoaderIsDisplayed();
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
            WaitWhilePreLoaderIsDisplayed();
        }

        private void SetPreferredLocationFilter(Tuple<string, string> preferredLocation)
        {
            WaitAndGetBySelector("categoryHolder", ApplicationSettings.TimeOut.Fast).Click();
            GetUIElements("preferredLocationCategory").First(x => x.GetAttribute("innerText").Equals(preferredLocation.Item1)).Click();
            WaitAndGetBySelector("locationHolder", ApplicationSettings.TimeOut.Fast).Click();
            GetUIElements("preferredLocation").First(x => x.GetAttribute("innerText").Equals(preferredLocation.Item2)).Click();
            WaitWhilePreLoaderIsDisplayed();
        }

        private void SetAmenitiesFilter(ICollection<string> amenityList)
        {
            WaitAndGetBySelector("amenitiesResetLink", ApplicationSettings.TimeOut.Fast).Click();
            GetUIElements("amenities").ForEach(x =>
                {
                    if (amenityList.Contains(x.GetAttribute("data-name")))
                        x.Click();
                    WaitWhilePreLoaderIsDisplayed();
                });
            WaitWhilePreLoaderIsDisplayed();
        }

        private void SetRatingRangeFilter(RatingRange ratingRange)
        {
            WaitAndGetBySelector("starRatingResetLink", ApplicationSettings.TimeOut.Fast).Click();
            WaitWhilePreLoaderIsDisplayed();
            GetUIElements("minStarRating")[ratingRange.From - 1].Click();
            WaitWhilePreLoaderIsDisplayed();
            if (ratingRange.From < 5)
                GetUIElements("maxStarRating")[ratingRange.To - 1].Click();
            WaitWhilePreLoaderIsDisplayed();
        }

        private void SetHotelNameFilter(string hotelName)
        {
            WaitAndGetBySelector("textHotelName", ApplicationSettings.TimeOut.Fast).SendKeys(hotelName);
            WaitAndGetBySelector("searchIcon", ApplicationSettings.TimeOut.Fast).Click();
            WaitWhilePreLoaderIsDisplayed();
        }
        #endregion
    }
}
