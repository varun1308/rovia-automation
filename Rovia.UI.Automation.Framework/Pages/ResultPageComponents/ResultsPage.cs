using System;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Framework.Configurations;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Framework.Pages
{
    public class ResultsPage:UIPage
    {
        #region Private Members

        private static int _currentPageNo = 1;

        #endregion

        #region Public Properties

        public IResultFilters ResultFilters { private get; set; }
        public IResultsHolder ResultsHolder { private get; set; }
        public IResultsTitle ResultTitle { private get; set; }

        #endregion

        #region Private Members

        private bool IsWaitingVisible()
        {
            var div = WaitAndGetBySelector("divWaiting", ApplicationSettings.TimeOut.Fast);
            return div != null && div.Displayed;
        }

        private void WaitWhilePreLoaderIsDisplayed()
        {
            var preloader = WaitAndGetBySelector("preLoader", ApplicationSettings.TimeOut.Fast);
            if(preloader!=null)
                while (preloader.Displayed) ;
        }

        private void GoToNextPage()
        {
            WaitAndGetBySelector("aNext", ApplicationSettings.TimeOut.Fast).Click();
            ++_currentPageNo;
            WaitWhilePreLoaderIsDisplayed();
            GetUIElements("divErrors").ForEach(x =>
            {
                if (x.Displayed)
                    throw new Alert(x.Text);
            });
        }

        private bool IsNextPageAvailable()
        {
            if (_currentPageNo >= ApplicationSettings.MaxSearchDepth)
                return false;
            var nextPageLink = WaitAndGetBySelector("aNext", ApplicationSettings.TimeOut.Fast);
            return nextPageLink!=null && nextPageLink.GetAttribute("href").EndsWith("ResultHolder");
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Add to cart a product
        /// </summary>
        /// <param name="criteria">Search Criteria Object</param>
        /// <returns></returns>
        public Results AddToCart(SearchCriteria criteria)
        {
            var selectedResult=ResultsHolder.AddToCart(criteria);
            if (selectedResult == null && IsNextPageAvailable())
            {
                GoToNextPage();
                selectedResult = AddToCart(criteria);
            }
            return selectedResult;
        }

        /// <summary>
        /// Wait for result to display
        /// </summary>
        public void WaitForResultLoad()
        {
            try
            {
                while (IsWaitingVisible())
                {
                    Thread.Sleep(2000);

                    if (ResultsHolder.IsVisible())
                        break;
                }
                GetUIElements("divErrors").ForEach(x =>
                    {
                        if(x.Displayed)
                            throw new Alert(x.Text);
                    });
                if (!ResultsHolder.IsVisible())
                    throw new Alert("Results Not visible");
                _currentPageNo = 1;
            }
            catch (Exception)
            {
                LogManager.GetInstance().LogInformation("Results Failed To Load");
                throw;
            }
        }

        /// <summary>
        /// set and validate the filters and matrix on results page
        /// </summary>
        /// <param name="postSearchFilters"></param>
        public void SetAndValidatePostSearchFilters(PostSearchFilters postSearchFilters)
        {
             ResultFilters.SetPostSearchFilters(postSearchFilters);
             ResultFilters.ValidateFilters(postSearchFilters);
        }
 
        /// <summary>
        /// Verify all the search filters passed during search
        /// </summary>
        /// <param name="preSearchFilters">Pre search filter object</param>
        public void VerifyPreSearchFilters(PreSearchFilters preSearchFilters)
        {
             ResultFilters.VerifyPreSearchFilters(preSearchFilters);
        }

        /// <summary>
        /// Validate search criteria with title on results page
        /// </summary>
        /// <param name="criteria"></param>
        public void ValidateSearch(SearchCriteria criteria)
        {
            if (!ResultTitle.ValidateTitle(criteria))
                throw new ValidationException("Invalid Results:");
        }

        #endregion

    }
}
