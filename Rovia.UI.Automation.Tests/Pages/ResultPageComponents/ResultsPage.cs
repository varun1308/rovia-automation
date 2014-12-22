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
            return (_currentPageNo < ApplicationSettings.MaxSearchDepth) && (WaitAndGetBySelector("aNext", ApplicationSettings.TimeOut.Fast).GetAttribute("href").EndsWith("ResultHolder"));
        }

        #endregion

        #region Public Members

        public Results AddToCart(string supplier)
        {
            var selectedResult=ResultsHolder.AddToCart(supplier);
            if (selectedResult == null && IsNextPageAvailable())
            {
                GoToNextPage();
                selectedResult = AddToCart(supplier);
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
            catch (Exception exception)
            {
                LogManager.GetInstance().LogInformation("Results Failed To Load");
                throw;// new Exception("Results failed to load", exception);
            }
        }

        public List<Results> ParseResults()
        {
            return ResultsHolder.ParseResults();
        }

        public void SetPostSearchFilters(PostSearchFilters postSearchFilters)
        {
             ResultFilters.SetPostSearchFilters(postSearchFilters);
        }
 
        public bool VerifyPreSearchFilters(PreSearchFilters preSearchFilters)
        {
            return ResultFilters.VerifyPreSearchFilters(preSearchFilters);
        }

        public void ValidateSearch(SearchCriteria criteria)
        {
            if (!ResultTitle.ValidateTitle(criteria))
                throw new ValidationException("Invalid Results:");
        }

        public void ValidateFilters(PostSearchFilters postSearchFilters)
        {
            ResultFilters.ValidateFilters(postSearchFilters, ResultsHolder.ParseResults());
        }

        #endregion

    }
}
