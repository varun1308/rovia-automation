using System;
using System.Collections.Generic;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public class ResultsPage:UIPage
    {
        #region Private Members

        private static IResultFilters _resultFilters;
        private static IResultsHolder _resultsHolder;
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
        }

        private bool IsNextPageAvailable()
        {
            return (_currentPageNo < ApplicationSettings.MaxSearchDepth) && (WaitAndGetBySelector("aNext", ApplicationSettings.TimeOut.Fast).GetAttribute("href").EndsWith("#AirResultHolder"));
        }

        #endregion

        #region Public Members

        public void Initialze(IResultsHolder resultHolder, IResultFilters resultFilter)
        {
            _resultsHolder = resultHolder;
            _resultFilters = resultFilter;
        }

        public Results AddToCart(string supplier)
        {
            var selectedResult=_resultsHolder.AddToCart(supplier);
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

                    if (_resultsHolder.IsVisible())
                        break;
                }
                GetUIElements("divErrors").ForEach(x =>
                    {
                        if(x.Displayed)
                            throw new Alert(x.Text);
                    });
                if (!_resultsHolder.IsVisible())
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
            return _resultsHolder.ParseResults();
        }

        public bool SetPostSearchFilters(PostSearchFilters postSearchFilters)
        {
         return _resultFilters.SetPostSearchFilters(postSearchFilters);
        }

        public bool VerifyPreSearchFilters(PreSearchFilters preSearchFilters)
        {
            return _resultFilters.VerifyPreSearchFilters(preSearchFilters);
        }
 
        #endregion
    }
}
