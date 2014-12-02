using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public class ResultsPage:UIPage
    {
        #region Private Members

        private static IResultFilters _resultFilters;
        private static IResultsHolder _resultsHolder;

        private bool IsWaitingVisible()
        {
            var div = WaitAndGetBySelector("divWaiting", ApplicationSettings.TimeOut.Fast);
            return div != null && div.Displayed;
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

        private void GoToNextPage()
        {
            WaitAndGetBySelector("aNext", ApplicationSettings.TimeOut.Fast).Click();
        }

        private bool IsNextPageAvailable()
        {
            return WaitAndGetBySelector("aNext", ApplicationSettings.TimeOut.Fast).GetAttribute("href").EndsWith("#AirResultHolder");
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
                GetUIElements("txtError").ForEach(x =>
                    {
                        if(x.Displayed)
                            throw new Alert(x.Text);
                    });
                if (!_resultsHolder.IsVisible())
                    throw new Alert("Results Not visible");
            }
            catch (Exception exception)
            {
                //todo log
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
 
        #endregion
    }
}
