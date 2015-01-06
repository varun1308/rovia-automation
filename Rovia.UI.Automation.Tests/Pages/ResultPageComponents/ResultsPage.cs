using System;
using System.Collections.Generic;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public class ResultsPage:UIPage
    {
        #region Private Members

        public IResultFilters ResultFilters { private get; set; }
        public IResultsHolder ResultsHolder { private get; set; }

        public IResultsTitle ResultTitle{ private get; set; }

        private static int _currentPageNo = 1;
        private bool IsWaitingVisible()
        {
            var div = WaitAndGetBySelector("divWaiting", ApplicationSettings.TimeOut.Fast);
            return div != null && div.Displayed;
        } 

        private void GoToNextPage()
        {
            WaitAndGetBySelector("aNext", ApplicationSettings.TimeOut.Fast).Click();
            ++_currentPageNo;
            Thread.Sleep(3000);
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

        public List<Results> ParseResults()
        {
            return ResultsHolder.ParseResults();
        }

        public void SetAndValidatePostSearchFilters(PostSearchFilters postSearchFilters)
        {
             ResultFilters.SetPostSearchFilters(postSearchFilters);
             ResultFilters.ValidateFilters(postSearchFilters,ResultsHolder.ParseResults);
        }
 
        public void VerifyPreSearchFilters(PreSearchFilters preSearchFilters)
        {
             ResultFilters.VerifyPreSearchFilters(preSearchFilters, ResultsHolder.ParseResults);
        }

        public void ValidateSearch(SearchCriteria criteria)
        {
            if (!ResultTitle.ValidateTitle(criteria))
                throw new ValidationException("Invalid Results:");
        }

        #endregion

    }
}
