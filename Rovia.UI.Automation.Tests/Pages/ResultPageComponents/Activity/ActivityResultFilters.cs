namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using AppacitiveAutomationFramework;
    using Exceptions;
    using Logger;
    using ScenarioObjects;
    using Configuration;

    /// <summary>
    /// Activity results page filters container
    /// </summary>
    class ActivityResultFilters : UIPage, IResultFilters
    {
        #region Private Fileds

        private List<string> _appliedFilters;
        private List<string> _failedFilters;
        
        #endregion

        #region Private Members

        private bool AreResultsAvailable()
        {
            foreach (var error in GetUIElements("error").Where(error => error.Displayed))
            {
                LogManager.GetInstance().LogWarning(error.Text);
                return false;
            }
            return true;
        }

        private void WaitWhilePreLoaderIsDisplayed()
        {
            var preloader = WaitAndGetBySelector("preLoader", ApplicationSettings.TimeOut.Fast);
            while (preloader.Displayed) ;
        }

        private IEnumerable<string> GetAppliedFilters()
        {
            Thread.Sleep(2000);
            return GetUIElements("appliedFilters").Select(x => x.Text.Trim());
        }

        #region Filter Setting Functions

        private void SetMatrix(ActivityMatrix matrix)
        {

            if (GetUIElements("matrixSection").Any(x =>
            {
                var category = x.WaitAndGetBySelector("matrixCategory", ApplicationSettings.TimeOut.Fast);
                if (!x.WaitAndGetBySelector("mCategoryName", ApplicationSettings.TimeOut.Fast).GetAttribute("data-id").Split('|')[0].Trim().Equals(matrix.Category, StringComparison.OrdinalIgnoreCase)) return false;
                category.Click();
                WaitWhilePreLoaderIsDisplayed();
                matrix.ItineraryCount = int.Parse(x.WaitAndGetBySelector("activityCount", ApplicationSettings.TimeOut.Fast).Text);
                matrix.StaringPrice = new Amount(x.WaitAndGetBySelector("startingPrice", ApplicationSettings.TimeOut.Fast).Text);
                return true;
            }))
                _appliedFilters.Add("Categories");
            else
                LogManager.GetInstance().LogWarning(string.Format("Matrix Filter was not applied( Requested Matrix Section with Category : {0} not Found )", matrix.Category));
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

        private void SetCategoriesFilter(IEnumerable<string> amenityList)
        {
            var categories = new List<string>(amenityList) { "all" };
            GetUIElements("categories").ForEach(x =>
            {
                if (categories.Contains(x.GetAttribute("data-code").Trim()))
                    x.Click();
                WaitWhilePreLoaderIsDisplayed();
            });
            WaitWhilePreLoaderIsDisplayed();
        }

        private void SetActivityNameFilter(string activityName)
        {
            WaitAndGetBySelector("textActivityName", ApplicationSettings.TimeOut.Fast).SendKeys(activityName);
            WaitAndGetBySelector("searchIcon", ApplicationSettings.TimeOut.Fast).Click();
            WaitWhilePreLoaderIsDisplayed();
        }

        private void SortResults(SortBy sortBy)
        {
            WaitAndGetBySelector("sortHeader", ApplicationSettings.TimeOut.Safe).Click();
            switch (sortBy)
            {
                case SortBy.PriceAsc: WaitAndGetBySelector("priceAsc", ApplicationSettings.TimeOut.Fast).Click();
                    break;
                case SortBy.PriceDsc: WaitAndGetBySelector("priceDsc", ApplicationSettings.TimeOut.Fast).Click();
                    break;
                case SortBy.NameAsc: WaitAndGetBySelector("nameAsc", ApplicationSettings.TimeOut.Fast).Click();
                    break;
                case SortBy.NameDsc: WaitAndGetBySelector("nameDsc", ApplicationSettings.TimeOut.Fast).Click();
                    break;
                case SortBy.CategoryAsc: WaitAndGetBySelector("categoryAsc", ApplicationSettings.TimeOut.Fast).Click();
                    break;
                case SortBy.CategoryDsc: WaitAndGetBySelector("categoryDsc", ApplicationSettings.TimeOut.Fast).Click();
                    break;
            }
            WaitWhilePreLoaderIsDisplayed();
        }

        #endregion

        #region Validation Functions

        private void ValidateMatrix(ActivityMatrix matrix)
        {
            var categories = GetUIElements("activityCategories").Select(x => x.Text.Remove(0, 11).Trim());
            var prices = GetUIElements("activityPrices").Select(x => new Amount(x.Text).TotalAmount);
            if (prices.First() < matrix.StaringPrice.TotalAmount || categories.Any(x => (x != matrix.Category)))
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
                case SortBy.CategoryAsc: ValidateCategorySort(sortBy);
                    break;
                case SortBy.CategoryDsc: ValidateCategorySort(sortBy);
                    break;
            }
            WaitWhilePreLoaderIsDisplayed();
        }

        private void ValidateCategorySort(SortBy order)
        {
            var categories = GetUIElements("activityCategories").Select(x => x.Text.Remove(0, 11).Trim()).ToArray();
            if (order == SortBy.CategoryDsc)
                Array.Reverse(categories);
            for (var i = 1; i < categories.Length; i++)
            {
                if (String.CompareOrdinal(categories[i].ToUpper(), categories[i - 1].ToUpper()) >= 0) continue;
                _failedFilters.Add("CategorySort");
                return;
            }
        }

        private void ValidateNameSort(SortBy order)
        {
            var activityNames = GetUIElements("activityNames").Select(x => x.Text).ToArray();
            if (order == SortBy.NameDsc)
                Array.Reverse(activityNames);
            for (var i = 1; i < activityNames.Length; i++)
            {
                if (String.CompareOrdinal(activityNames[i].ToUpper(), activityNames[i - 1].ToUpper()) >= 0) continue;
                _failedFilters.Add("ActivityNameSort");
                return;
            }
        }

        private void ValidatePriceSort(SortBy order)
        {
            var prices = GetUIElements("activityPrices").Select(x => new Amount(x.Text).TotalAmount).ToArray();
            if (order == SortBy.PriceDsc)
                Array.Reverse(prices);
            for (var i = 1; i < prices.Length; i++)
            {
                if (prices[i] >= prices[i - 1]) continue;
                _failedFilters.Add("PriceSort");
                return;
            }
        }

        private void ValidateActivityNameFilter(string activityName)
        {
            var activityNames = GetUIElements("activityNames").Select(x => x.Text);
            if (activityNames.All(x => x.Contains(activityName)))
                return;
            _failedFilters.Add("Activity Name");
        }

        private void ValidatePriceRangeFilter(PriceRange priceRange)
        {
            var prices = GetUIElements("activityPrices").Select(x => new Amount(x.Text).TotalAmount);
            if (prices.Any(x => x > priceRange.MaxPrice || x < priceRange.MinPrice))
                _failedFilters.Add("Price");
        }

        private void ValidateCategoryFilter(ICollection<string> filterCategories)
        {
            var categories = GetUIElements("activityCategories").Select(x => x.Text.Remove(0, 11).Trim());
            if (categories.All(filterCategories.Contains))
                return;
            _failedFilters.Add("Categories");
        }

        #endregion

        #endregion

        #region Public Members

        /// <summary>
        /// Verify pre search filters on resulting itineraries
        /// </summary>
        /// <param name="preSearchFilters">Applied pre search filter object</param>
        public void VerifyPreSearchFilters(PreSearchFilters preSearchFilters)
        {
            //No PreSearchFilters to verify
        }

        /// <summary>
        /// Set Filters and Matrix on result page
        /// </summary>
        /// <param name="postSearchFilters">Filters and matrix parameter to set</param>
        public void SetPostSearchFilters(PostSearchFilters postSearchFilters)
        {
            var activityPostSearchFilters = postSearchFilters as ActivityPostSearchFilters;
            if (activityPostSearchFilters == null)
                throw new InvalidInputException("PostSearchFilters");
            _appliedFilters = new List<string>();
            if (activityPostSearchFilters.Matrix != null)
            {
                SetMatrix(activityPostSearchFilters.Matrix as ActivityMatrix);
                if (!AreResultsAvailable())
                    activityPostSearchFilters.Matrix = null;
            }
            if (activityPostSearchFilters.PriceRange != null)
            {
                SetPriceRangeFilter(activityPostSearchFilters.PriceRange);
                if (!AreResultsAvailable())
                    activityPostSearchFilters.PriceRange = null;
                else
                    _appliedFilters.Add("Price");
            }
            if (activityPostSearchFilters.ActivityName != null)
            {
                SetActivityNameFilter(activityPostSearchFilters.ActivityName);
                if (!AreResultsAvailable())
                    activityPostSearchFilters.ActivityName = null;
                else
                    _appliedFilters.Add("Activity Name");
            }
            if (activityPostSearchFilters.Categories != null)
            {
                SetCategoriesFilter(activityPostSearchFilters.Categories);
                if (!AreResultsAvailable())
                    activityPostSearchFilters.Categories = null;
                else
                    _appliedFilters.Add("Categories");
            }
            if (activityPostSearchFilters.SortBy != SortBy.Featured)
            {
                SortResults(activityPostSearchFilters.SortBy);
            }
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
            var activityPostSearchFilters = postSearchFilters as ActivityPostSearchFilters;
            _failedFilters = new List<string>();
            if (activityPostSearchFilters == null)
                throw new InvalidInputException("PostSearchFilters");
            if (activityPostSearchFilters.PriceRange != null)
                ValidatePriceRangeFilter(activityPostSearchFilters.PriceRange);
            if (activityPostSearchFilters.ActivityName != null)
                ValidateActivityNameFilter(activityPostSearchFilters.ActivityName);
            if (activityPostSearchFilters.Categories != null)
                ValidateCategoryFilter(activityPostSearchFilters.Categories);
            if (activityPostSearchFilters.SortBy != SortBy.PriceAsc)
                ValidateSort(activityPostSearchFilters.SortBy);
            if (activityPostSearchFilters.Matrix != null)
            {
                if (activityPostSearchFilters.SortBy != SortBy.PriceAsc)
                    SortResults(SortBy.PriceAsc);
                ValidateMatrix(activityPostSearchFilters.Matrix as ActivityMatrix);
                if (activityPostSearchFilters.SortBy != SortBy.PriceAsc)
                    SortResults(activityPostSearchFilters.SortBy);
            }
            if (_failedFilters.Any())
                throw new ValidationException("Validation Failed for following filters : " + string.Join(",", _failedFilters));
        }

        #endregion
       
    }
}
