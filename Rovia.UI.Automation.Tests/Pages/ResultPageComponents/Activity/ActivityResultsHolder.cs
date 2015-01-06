using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppacitiveAutomationFramework;
using OpenQA.Selenium;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Pages.ResultPageComponents.Activity;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public class ActivityResultsHolder : UIPage, IResultsHolder
    {
        private Results _selectedItinerary;
        public ActivityHolder ActivityHolder { private get; set; }

        private void SelectActivity(Passengers passengers)
        {
            try
            {
                //var suppliers = GetUIElements("suppliers").Where((x, i) => i % 3 == 2).Select(x => x.GetAttribute("title").Split('|')).ToArray();
                var validIndices = Enumerable.Range(0, 10);
                //if (!string.IsNullOrEmpty(supplier))
                //    validIndices = validIndices.Where(x => suppliers[x][1].Equals(hotelSupplier));
                var resultIndex = validIndices.First(i => AddActivity(GetUIElements("btnSelectActivity")[i], passengers));
            }
            catch (StaleElementReferenceException)
            {
                LogManager.GetInstance().LogWarning("StaleElementReferenceException suppressed.");
            }
        }

        private bool AddActivity(IUIWebElement btnSelectActivity, Passengers passengers)
        {
            try
            {
                btnSelectActivity.Click();
                ActivityHolder.WaitForLoad();
                _selectedItinerary = ActivityHolder.AddToCart(passengers);
                return true;
            }
            catch (Alert alert)
            {
                LogManager.GetInstance().LogWarning(alert.Message);
                throw new AddToCartFailedException();
            }
        }

        public bool IsVisible()
        {
            var div = WaitAndGetBySelector("divResultHolder", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }

        public List<Results> ParseResults()
        {
            throw new NotImplementedException();
        }

        public Results AddToCart(SearchCriteria  criteria)
        {
            try
            {
                SelectActivity(criteria.Passengers);
                return _selectedItinerary;
            }
            catch (System.InvalidOperationException)
            {
                LogManager.GetInstance().LogWarning("No suitable results found on this page");
                return null;
            }
        }

    }
}
