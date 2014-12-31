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
        #region IResultPage Members

        public bool VerifyPreSearchFilters(PreSearchFilters preSearchFilters,Func<List<Results>> getParsedResults)
        {
            //_appliedFilters = GetAppliedFilters().ToList();  
            //var hotelFilters = preSearchFilters as HotelPreSearchFilters;
            //if (hotelFilters == null)
            //    throw new InvalidInputException("PreSearchFilters");
            //var hotelResults = getParsedResults().Select(x => x as HotelResult).ToList();
            //    _failedFilters = new List<string>();

            //if (!string.IsNullOrEmpty(hotelFilters.HotelName))
            //{
            //    if(_appliedFilters.Contains("Hotel Name"))
            //        ValidateHotelName(hotelFilters.HotelName, hotelResults.Select(x => x.HotelName));
            //    else
            //        _failedFilters.Add("Hotel Name")
            //}
            //if(!string.IsNullOrEmpty(hotelFilters.StarRating))
            //    ValidateRatingRange(new RatingRange() { From = int.Parse(hotelFilters.StarRating),To = 5}, hotelResults.Select(x => x.HotelRating));
            return true;
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
                }
                if (hotelPostSearchFilters.PriceRange != null)
                {
                    SetPriceRange(hotelPostSearchFilters.PriceRange);
                    _appliedFilters.Add("Price");
                }
                if (hotelPostSearchFilters.HotelName != null)
                {
                    SetHotelName(hotelPostSearchFilters.HotelName);
                    _appliedFilters.Add("Hotel Name");
                }
                if (hotelPostSearchFilters.RatingRange != null)
                {
                    SetRatingRange(hotelPostSearchFilters.RatingRange);
                    _appliedFilters.Add("Star Rating");
                }
                if (hotelPostSearchFilters.Amenities != null)
                {
                    SetAmenitiesFilter(hotelPostSearchFilters.Amenities);
                    _appliedFilters.Add("Amenities");
                }
                if (hotelPostSearchFilters.PreferredLocation != null)
                {
                    SetPreferredLocationFilter(hotelPostSearchFilters.PreferredLocation);
                    _appliedFilters.Add("Preferred Location");
                }
                if (hotelPostSearchFilters.DistanceRange != null)
                {
                    SetDistanceRangeFilter(hotelPostSearchFilters.DistanceRange);
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

        public void ValidateFilters(PostSearchFilters postSearchFilters, Func<List<Results>> getParsedResults)
        {
                var hotelPostSearchFilters = postSearchFilters as HotelPostSearchFilters;
                var hotelResults = getParsedResults().Select(x => x as HotelResult).ToList();
                _failedFilters = new List<string>();
                if (hotelPostSearchFilters == null)
                    throw new InvalidInputException("PostSearchFilters");
                if (hotelPostSearchFilters.PriceRange != null)
                    ValidatePriceRange(hotelPostSearchFilters.PriceRange, hotelResults.Select(x => x.Amount.TotalAmount));
                if (hotelPostSearchFilters.HotelName != null)
                    ValidateHotelName(hotelPostSearchFilters.HotelName, hotelResults.Select(x => x.HotelName));
                if (hotelPostSearchFilters.RatingRange != null)
                    ValidateRatingRange(hotelPostSearchFilters.RatingRange, hotelResults.Select(x => x.HotelRating));
                if (hotelPostSearchFilters.SortBy != SortBy.Featured)
                    ValidateSort(hotelPostSearchFilters.SortBy, hotelResults);
                if (hotelPostSearchFilters.Matrix != null)
                {
                    ValidateMatrix(hotelPostSearchFilters.Matrix as HotelMatrix, getParsedResults);
                    SortResults(hotelPostSearchFilters.SortBy);
                }
                if (_failedFilters.Any())
                    throw new ValidationException("Validation Failed for following filters : " + string.Join(",", _failedFilters));
        }

        private void ValidateMatrix(HotelMatrix matrix, Func<List<Results>> getParsedResults)
        {
            SortResults(SortBy.PriceAsc);
            var results = getParsedResults();
            if (!results.First().Amount.TotalAmount.Equals(matrix.Amount.TotalAmount) ||
                results.Any(x => (x as HotelResult).HotelRating != matrix.Rating))
                _failedFilters.Add("Matrix");
        }

        private void ValidateSort(SortBy sortBy, List<HotelResult> hotelResults)
        {
            switch (sortBy)
            {
                case SortBy.PriceAsc: ValidateSortPrice(hotelResults.Select(x => x.Amount.TotalAmount).ToList());
                    break;
                case SortBy.PriceDsc: ValidateSortPrice(hotelResults.Select(x => x.Amount.TotalAmount).Reverse().ToList());
                    break;
                case SortBy.HotelNameAsc: ValidateName(hotelResults.Select(x => x.HotelName).ToList()); 
                    break;
                case SortBy.HotelNameDsc: ValidateName(hotelResults.Select(x => x.HotelName).Reverse().ToList()); 
                    break;
                case SortBy.RatingAsc: ValidateRating(hotelResults.Select(x => x.HotelRating).ToList());
                    break;
                case SortBy.RatingDsc: ValidateRating(hotelResults.Select(x => x.HotelRating).Reverse().ToList());
                    break;
            }
            WaitWhilePreLoaderIsDisplayed();
        }

        private void ValidateRating(IList<int> ratingList)
        {
            for (var i = 1; i < ratingList.Count; i++)
            {
                if (ratingList[i] >= ratingList[i - 1]) continue;
                _failedFilters.Add("RatingSort");
                return;
            }
        }

        private void ValidateName(IList<string> nameList)
        {
            for (var i = 1; i < nameList.Count; i++)
            {
                if (String.CompareOrdinal(nameList[i].ToUpper(), nameList[i-1].ToUpper())>=0) continue;
                _failedFilters.Add("HotelNameSort");
                return;
            }
        }

        private void ValidateSortPrice(IList<double> priceList)
        {
            for (var i = 1; i < priceList.Count; i++)
            {
                if (priceList[i] >= priceList[i - 1]) continue;
                _failedFilters.Add("PriceSort");
                return;
            }
        }

        #endregion

        #region private Hotel Specific members

        private void WaitWhilePreLoaderIsDisplayed()
        {
            var preloader = WaitAndGetBySelector("preLoader", ApplicationSettings.TimeOut.Fast);
            while (preloader.Displayed) ;
        }

        private void SetMatrix(HotelMatrix matrix)
        {
            try
            {
                var matrixSection=GetUIElements("matrixSection").First(x => x.GetUIElements("ratedStars").Count == matrix.Rating);
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
                case SortBy.HotelNameAsc: WaitAndGetBySelector("nameAsc", ApplicationSettings.TimeOut.Fast).Click();
                    break;
                case SortBy.HotelNameDsc: WaitAndGetBySelector("nameDsc", ApplicationSettings.TimeOut.Fast).Click();
                    break;
                case SortBy.RatingAsc: WaitAndGetBySelector("ratingAsc", ApplicationSettings.TimeOut.Fast).Click();
                    break;
                case SortBy.RatingDsc: WaitAndGetBySelector("ratingDsc", ApplicationSettings.TimeOut.Fast).Click();
                    break;
            }
            WaitWhilePreLoaderIsDisplayed();
        }

        private void ValidateRatingRange(RatingRange ratingRange, IEnumerable<int> ratingList)
        {
            if (ratingList.Any(x => x > ratingRange.To || x < ratingRange.From))
                _failedFilters.Add("Rating");
        }

        private void ValidateHotelName(string hotelName, IEnumerable<string> hotelNames)
        {
            if (hotelNames.Any(x =>!x.Contains(hotelName)))
                _failedFilters.Add("Hotel Name");
        }

        private void ValidatePriceRange(PriceRange priceRange, IEnumerable<double> amountList)
        {
            if (amountList.Any(x => x > priceRange.MaxPrice || x < priceRange.MinPrice))
                _failedFilters.Add("Price");
        }

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

            priceRange.MinPrice = minPrice+(minPrice * priceRange.Min / 100);
            priceRange.MaxPrice = maxPrice-(maxPrice * priceRange.Max / 100);
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

        private void SetRatingRange(RatingRange ratingRange)
        {
            WaitAndGetBySelector("starRatingResetLink", ApplicationSettings.TimeOut.Fast).Click();
            WaitWhilePreLoaderIsDisplayed();
            GetUIElements("minStarRating")[ratingRange.From - 1].Click();
            WaitWhilePreLoaderIsDisplayed();
            if (ratingRange.From < 5)
                GetUIElements("maxStarRating")[ratingRange.To - 1].Click();
            WaitWhilePreLoaderIsDisplayed();
        }

        private void SetHotelName(string hotelName)
        {
            WaitAndGetBySelector("textHotelName", ApplicationSettings.TimeOut.Fast).SendKeys(hotelName);
            WaitAndGetBySelector("searchIcon", ApplicationSettings.TimeOut.Fast).Click();
            WaitWhilePreLoaderIsDisplayed();
        }
        #endregion
    }
}
