﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Framework.Configurations;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Framework.Pages
{
    /// <summary>
    /// Rovia site specific car product search methods
    /// </summary>
    public class CarSearchPanel : UIPage, ISearchPanel
    {
        #region Protected Members

        protected void ApplyDiscountCode(List<CorporateDiscount> corporateDiscounts)
        {
            ExecuteJavascript("$('span:contains(\"Corporate Discount\")').click()");
            foreach (var corpDisc in corporateDiscounts)
            {
                WaitAndGetBySelector("selectCorpDiscCodeRentalAgency", ApplicationSettings.TimeOut.Slow).SelectFromDropDown(corpDisc.RentalAgency);
                WaitAndGetBySelector("txtcorporateDiscountCode", ApplicationSettings.TimeOut.Fast).SendKeys(corpDisc.CorpDiscountCode);
                WaitAndGetBySelector("txtPromotionalCOde", ApplicationSettings.TimeOut.Fast).SendKeys(corpDisc.PromotionalCode);
                WaitAndGetBySelector("btnaddCorporateCode", ApplicationSettings.TimeOut.Fast).Click();
            }
            ExecuteJavascript("$('span:contains(\"Corporate Discount\")').click()");
        }

        protected void SetOriginLocation(string location)
        {
            IUIWebElement autoSuggestBox;
            do
            {
                autoSuggestBox = WaitAndGetBySelector("autoSuggestBoxOriginLoc", ApplicationSettings.TimeOut.Fast);
            } while (autoSuggestBox == null || !autoSuggestBox.Displayed);
            GetUIElements("autoSuggestOptionsOriginLoc").First(x => (x.Displayed && x.Text.Equals(location))).Click();
        }

        protected void SetDestinationLocation(string location)
        {
            //to implement
            //IUIWebElement autoSuggestBox;
            //do
            //{
            //    autoSuggestBox = WaitAndGetBySelector("autoSuggestBoxDestinationLoc", ApplicationSettings.TimeOut.Fast);
            //} while (autoSuggestBox == null || !autoSuggestBox.Displayed);
            //GetUIElements("autoSuggestOptionsDestinationLoc").First(x => (x.Displayed && x.Text.Equals(location))).Click();
        }

        protected void ResolveMultiLocationOptions()
        {
            var multiLocOption = WaitAndGetBySelector("multiLocOptionButton", ApplicationSettings.TimeOut.Fast);
            if (multiLocOption != null && multiLocOption.Displayed)
                multiLocOption.Click();
        }

        protected virtual void EnterPickUpDetails(CarSearchCriteria carSearchCriteria)
        {
            if (carSearchCriteria.PickUp.PickUpType.Equals(PickUpType.Airport))
            {
                WaitAndGetBySelector("pickUpLoc", ApplicationSettings.TimeOut.Slow).SendKeys(carSearchCriteria.PickUp.PickUpLocCode);
                if (!string.IsNullOrEmpty(carSearchCriteria.PickUp.PickUpLocation)) SetOriginLocation(carSearchCriteria.PickUp.PickUpLocation);
            }
            else
            {
                WaitAndGetBySelector("pickUpCity", ApplicationSettings.TimeOut.Slow).Click();
                WaitAndGetBySelector("pickUpCityLoc", ApplicationSettings.TimeOut.Slow).SendKeys(carSearchCriteria.PickUp.PickUpLocCode);
                if (!string.IsNullOrEmpty(carSearchCriteria.PickUp.PickUpLocation)) SetOriginLocation(carSearchCriteria.PickUp.PickUpLocation);
            }
            var pickUpDate = WaitAndGetBySelector("pickUpDate", ApplicationSettings.TimeOut.Slow);
            pickUpDate.SendKeys(carSearchCriteria.PickUp.PickUpDate.ToString("MM/dd/yyyy"));
            pickUpDate.Click();
            WaitAndGetBySelector("pickUpTime", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(carSearchCriteria.PickUp.PickUpTime);
        }

        protected virtual void EnterDropOffDetails(CarSearchCriteria carSearchCriteria)
        {
            if (!carSearchCriteria.DropOff.DropOffType.Equals(DropOffType.SameAsPickUp))
            {
                if (carSearchCriteria.DropOff.DropOffType.Equals(DropOffType.Airport))
                    WaitAndGetBySelector("dropoffLoc", ApplicationSettings.TimeOut.Slow)
                        .SendKeys(carSearchCriteria.DropOff.DropOffLocCode);
                else
                {
                    WaitAndGetBySelector("dropoffCity", ApplicationSettings.TimeOut.Slow).Click();
                    WaitAndGetBySelector("dropoffCityLoc", ApplicationSettings.TimeOut.Slow)
                        .SendKeys(carSearchCriteria.DropOff.DropOffLocCode);
                }
                if (!string.IsNullOrEmpty(carSearchCriteria.DropOff.DropOffLocation))
                    SetDestinationLocation(carSearchCriteria.DropOff.DropOffLocation);
            }
            else
                WaitAndGetBySelector("dropoffSameAsPickUp", ApplicationSettings.TimeOut.Slow).Click();
            var dropoffDate = WaitAndGetBySelector("dropoffDate", ApplicationSettings.TimeOut.Slow);
            dropoffDate.SendKeys(carSearchCriteria.DropOff.DropOffDate.ToString("MM/dd/yyyy"));
            dropoffDate.Click();
            WaitAndGetBySelector("dropoffTime", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(carSearchCriteria.DropOff.DropOffTime);
        }
        
        protected void ApplyPreSearchFilters(PreSearchFilters filters)
        {
            var carFilters = filters as CarPreSearchFilters;
            if (!string.IsNullOrEmpty(carFilters.RentalAgency))
                ExecuteJavascript("$('#ulRentalCompany').find('[data-value=\"" + carFilters.RentalAgency +
                                  "\"]').click()");

            if (!string.IsNullOrEmpty(carFilters.CarType))
                ExecuteJavascript("$('#ulCarType').find('[data-value=\"" + carFilters.CarType +
                                      "\"]').click()");

            ExecuteJavascript("$('#ulAirConditioning').find('[data-value=\"" + carFilters.AirConditioning +
                               "\"]').click()");

            ExecuteJavascript("$('#ulTransmission').find('[data-value=\"" + carFilters.Transmission +
                              "\"]').click()");
            if (!string.IsNullOrEmpty(carFilters.CorporateDiscount[0].RentalAgency))
                ApplyDiscountCode(carFilters.CorporateDiscount);
        }

        protected void SelectSearchPanel()
        {
            var navBar = WaitAndGetBySelector("navBar", ApplicationSettings.TimeOut.Slow);
            if (navBar == null || !navBar.Displayed)
                throw new UIElementNullOrNotVisible("Navigation Bar ");
            navBar.Click();

            Thread.Sleep(500);

            var searchPanel = WaitAndGetBySelector("searchPanel", ApplicationSettings.TimeOut.Slow);
            if (searchPanel == null || !searchPanel.Displayed)
                throw new UIElementNullOrNotVisible("SearchPanel");
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Search for car product
        /// </summary>
        /// <param name="searchCriteria">Car Search Criteria Object</param>
        public void Search(SearchCriteria searchCriteria)
        {
            var carSearchCriteria = searchCriteria as CarSearchCriteria;
            SelectSearchPanel();
            EnterPickUpDetails(carSearchCriteria);
            EnterDropOffDetails(carSearchCriteria);
            ApplyPreSearchFilters(carSearchCriteria.Filters.PreSearchFilters);
            WaitAndGetBySelector("buttonCarSearch", ApplicationSettings.TimeOut.Slow).Click();
            ResolveMultiLocationOptions();
        }

        #endregion
    }
}