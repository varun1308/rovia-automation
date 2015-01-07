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
using Rovia.UI.Automation.ScenarioObjects.Activity;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Pages.ResultPageComponents.Activity;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public class ActivityResultsHolder : UIPage, IResultsHolder
    {
        private Results _selectedItinerary;
        public ActivityHolder ActivityHolder { private get; set; }

        private void SelectActivity(SearchCriteria criteria)
        {
            try
            {
                //var suppliers = GetUIElements("suppliers").Where((x, i) => i % 3 == 2).Select(x => x.GetAttribute("title").Split('|')).ToArray();
                var validIndices = Enumerable.Range(0, 10);
                //if (!string.IsNullOrEmpty(supplier))
                //    validIndices = validIndices.Where(x => suppliers[x][1].Equals(hotelSupplier));
                var resultIndex = validIndices.First(i => AddActivity(GetUIElements("btnSelectActivity")[i], criteria));
            }
            catch (StaleElementReferenceException)
            {
                LogManager.GetInstance().LogWarning("StaleElementReferenceException suppressed.");
            }
        }

        private bool AddActivity(IUIWebElement btnSelectActivity, SearchCriteria criteria)
        {
            try
            {
                btnSelectActivity.Click();
                ActivityHolder.WaitForLoad();
                _selectedItinerary = ActivityHolder.AddToCart(criteria as ActivitySearchCriteria);
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
            var activityNames = GetUIElements("activityNames").Select(x => x.Text);
            var categories = GetUIElements("activityCategories").Select(x => x.Text.Remove(0, 11).Trim());
            var prices = GetUIElements("activityPrices").Select(x => new Amount(x.Text));
            return activityNames.Select((x, i) =>
                new ActivityResult()
                    {
                        Name = x,
                        Amount = prices.ElementAt(i),
                        Category = categories.ElementAt(i)
                    } as Results
                ).ToList();
        }

        public Results AddToCart(SearchCriteria criteria)
        {
            try
            {
                SelectActivity(criteria);
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
