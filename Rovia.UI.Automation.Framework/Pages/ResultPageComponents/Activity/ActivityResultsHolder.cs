using System.Linq;
using AppacitiveAutomationFramework;
using OpenQA.Selenium;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Framework.Configurations;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Framework.Pages
{
    /// <summary>
    /// Activity Results page itineraries container
    /// </summary>
    public class ActivityResultsHolder : UIPage, IResultsHolder
    {
        #region Private Fields

        private Results _selectedItinerary;
        public ActivityHolder ActivityHolder { private get; set; }

        #endregion

        #region Private Members

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

        #endregion

        #region Public Members

        /// <summary>
        /// Check if results visible
        /// </summary>
        /// <returns></returns>
        public bool IsVisible()
        {
            var div = WaitAndGetBySelector("divResultHolder", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }
        
        /// <summary>
        /// Add activity product to cart
        /// </summary>
        /// <param name="criteria">criteria object to add itinerary</param>
        /// <returns></returns>
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

        #endregion
    }
}
