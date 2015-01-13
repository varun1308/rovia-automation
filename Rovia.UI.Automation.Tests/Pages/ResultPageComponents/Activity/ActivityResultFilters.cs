using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.ScenarioObjects.Activity;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    class ActivityResultFilters : UIPage, IResultFilters
    {
        private List<string> _appliedFilters;
        private List<string> _failedFilters;

        #region IResultPage Members

        public void VerifyPreSearchFilters(PreSearchFilters preSearchFilters, Func<List<Results>> getParsedResults)
        {
            //No PreSearchFilters to verify
            return;
        }

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

        public void ValidateFilters(PostSearchFilters postSearchFilters, Func<List<Results>> getParsedResults)
        {
            var activityPostSearchFilters = postSearchFilters as ActivityPostSearchFilters;
            var activityResults = getParsedResults().Select(x => x as ActivityResult).ToList();
            _failedFilters = new List<string>();
            if (activityPostSearchFilters == null)
                throw new InvalidInputException("PostSearchFilters");
            if (activityPostSearchFilters.PriceRange != null)
                ValidatePriceRangeFilter(activityPostSearchFilters.PriceRange, activityResults.Select(x => x.Amount.TotalAmount));
            if (activityPostSearchFilters.ActivityName != null)
                ValidateActivityNameFilter(activityPostSearchFilters.ActivityName, activityResults.Select(x => x.Name));
            if (activityPostSearchFilters.Categories != null)
                ValidateCategoryFilter(activityPostSearchFilters.Categories, activityResults.Select(x => x.Category));
            if (activityPostSearchFilters.SortBy != SortBy.PriceAsc)
                ValidateSort(activityPostSearchFilters.SortBy, activityResults);
            if (activityPostSearchFilters.Matrix != null)
            {
                if(activityPostSearchFilters.SortBy!=SortBy.PriceAsc)
                    SortResults(SortBy.PriceAsc);
                ValidateMatrix(activityPostSearchFilters.Matrix as ActivityMatrix, getParsedResults);
                if(activityPostSearchFilters.SortBy!=SortBy.PriceAsc)
                    SortResults(activityPostSearchFilters.SortBy);
            }
            if (_failedFilters.Any())
                throw new ValidationException("Validation Failed for following filters : " + string.Join(",", _failedFilters));
        }

        #endregion

        #region private Hotel Specific members

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

        private void ValidateMatrix(ActivityMatrix matrix, Func<List<Results>> getParsedResults)
        {
            var results = getParsedResults();
            if (results.First().Amount.TotalAmount<matrix.StaringPrice.TotalAmount ||
               results.Any(x => (x as ActivityResult).Category != matrix.Category))
                _failedFilters.Add("Matrix");
        }

        private void ValidateSort(SortBy sortBy, IEnumerable<ActivityResult> activityResults)
        {
            switch (sortBy)
            {
                case SortBy.PriceAsc: ValidatePriceSort(activityResults.Select(x => x.Amount.TotalAmount).ToList());
                    break;
                case SortBy.PriceDsc: ValidatePriceSort(activityResults.Select(x => x.Amount.TotalAmount).Reverse().ToList());
                    break;
                case SortBy.NameAsc: ValidateNameSort(activityResults.Select(x => x.Name).ToList());
                    break;
                case SortBy.NameDsc: ValidateNameSort(activityResults.Select(x => x.Name).Reverse().ToList());
                    break;
                case SortBy.CategoryAsc: ValidateCategorySort(activityResults.Select(x => x.Category).ToList());
                    break;
                case SortBy.CategoryDsc: ValidateCategorySort(activityResults.Select(x => x.Category).Reverse().ToList());
                    break;
            }
            WaitWhilePreLoaderIsDisplayed();
        }

        private void ValidateCategorySort(IList<string> categoryList)
        {
            for (var i = 1; i < categoryList.Count; i++)
            {
                if (String.CompareOrdinal(categoryList[i].ToUpper(), categoryList[i - 1].ToUpper()) >= 0) continue;
                _failedFilters.Add("CategorySort");
                return;
            }
        }

        private void ValidateNameSort(IList<string> nameList)
        {
            for (var i = 1; i < nameList.Count; i++)
            {
                if (String.CompareOrdinal(nameList[i].ToUpper(), nameList[i - 1].ToUpper()) >= 0) continue;
                _failedFilters.Add("ActivityNameSort");
                return;
            }
        }

        private void ValidatePriceSort(IList<double> priceList)
        {
            for (var i = 1; i < priceList.Count; i++)
            {
                if (priceList[i] >= priceList[i - 1]) continue;
                _failedFilters.Add("PriceSort");
                return;
            }
        }

        private void ValidateActivityNameFilter(string activityName, IEnumerable<string> activityNames)
        {
            if (activityNames.Any(x => !x.Contains(activityName)))
                _failedFilters.Add("Activity Name");
        }

        private void ValidatePriceRangeFilter(PriceRange priceRange, IEnumerable<double> amountList)
        {
            if (amountList.Any(x => x > priceRange.MaxPrice || x < priceRange.MinPrice))
                _failedFilters.Add("Price");
        }

        private void ValidateCategoryFilter(ICollection<string> filterCategories, IEnumerable<string> resultCategories)
        {
            if (resultCategories.All(filterCategories.Contains))
                return;
            _failedFilters.Add("Categories");
        }

        private void SetMatrix(ActivityMatrix matrix)
        {

            if (GetUIElements("matrixSection").Any(x =>
                {
                    var category = x.WaitAndGetBySelector("matrixCategory", ApplicationSettings.TimeOut.Fast);
                    if (!x.WaitAndGetBySelector("mCategoryName",ApplicationSettings.TimeOut.Fast).GetAttribute("data-id").Split('|')[0].Trim().Equals(matrix.Category, StringComparison.OrdinalIgnoreCase)) return false;
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
        #endregion
    }
}
