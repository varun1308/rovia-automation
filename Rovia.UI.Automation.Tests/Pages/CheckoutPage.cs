using System;
using AppacitiveAutomationFramework;
using OpenQA.Selenium;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages
{
    public class CheckoutPage : UIPage
    {
        #region Private Members

        private void SetPaymentMode(PaymentMode paymentMode)
        {
            var payOption = WaitAndGetBySelector(paymentMode == PaymentMode.RoviaBucks ? "roviaBucksCheck" : "creditCardCheck", ApplicationSettings.TimeOut.Fast);
            if (!payOption.Selected)
                payOption.Click();
        }

        private void WaitForPayment()
        {
            try
            {
                IUIWebElement preLoadingDiv;
                do
                {
                    preLoadingDiv = WaitAndGetBySelector("preloading", ApplicationSettings.TimeOut.Fast);
                } while (preLoadingDiv != null && preLoadingDiv.Displayed);
            }
            catch (StaleElementReferenceException exception)
            {
                LogManager.GetInstance().LogInformation("StaleElementReferenceException caught and suppressed");
                //eat OpenQASelenium.StaleElementReferenceException 
            }

        }

        private void SetAddress(BillingAddress billingAddress)
        {
            try
            {
                WaitAndGetBySelector("typeRB", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(billingAddress.Type);
                WaitAndGetBySelector("address1", ApplicationSettings.TimeOut.Fast).SendKeys(billingAddress.Line1);
                WaitAndGetBySelector("address2", ApplicationSettings.TimeOut.Fast).SendKeys(billingAddress.Line2);
                WaitAndGetBySelector("address_Country", ApplicationSettings.TimeOut.Fast)
                    .SelectFromDropDown(billingAddress.Country);

                if (billingAddress.Country.Equals("United States"))
                    WaitAndGetBySelector("address_drpState", ApplicationSettings.TimeOut.Fast)
                        .SelectFromDropDown(billingAddress.State);
                else
                    WaitAndGetBySelector("address_txtState", ApplicationSettings.TimeOut.Fast).SendKeys(billingAddress.State);

                WaitAndGetBySelector("address_txtCity", ApplicationSettings.TimeOut.Fast).SendKeys(billingAddress.City);

                WaitAndGetBySelector("address_txtZipcode", ApplicationSettings.TimeOut.Fast).SendKeys(billingAddress.ZipCode);

                WaitAndGetBySelector("address_phnArea", ApplicationSettings.TimeOut.Fast)
                    .SendKeys(billingAddress.PhoneNumber.Substring(0, 3));
                WaitAndGetBySelector("address_phnExchange", ApplicationSettings.TimeOut.Fast)
                    .SendKeys(billingAddress.PhoneNumber.Substring(3, 3));
                WaitAndGetBySelector("address_phnDigits", ApplicationSettings.TimeOut.Fast)
                    .SendKeys(billingAddress.PhoneNumber.Substring(6, 4));
            }
            catch (Exception exception)
            {
                LogManager.GetInstance().LogInformation("Set Address Failed");
                throw;
            }
        }

        private void CheckAndThrowErrors()
        {
            var divErrors = WaitAndGetBySelector("divErrors", ApplicationSettings.TimeOut.Slow);
            if (divErrors != null && divErrors.Displayed)
                throw new Alert(divErrors.Text);
        }

        #endregion

        #region Protected Members

        internal void PayNow(PaymentMode paymentMode)
        {
            SetPaymentMode(paymentMode);
            WaitAndGetBySelector("checkTerms", ApplicationSettings.TimeOut.Fast).Click();
            GetUIElements("paynow")[1].Click();
            WaitForPayment();
            CheckAndThrowErrors();
        }

        internal void WaitForLoad()
        {
            IUIWebElement fillCcDetailsDiv;
            do
            {
                fillCcDetailsDiv = WaitAndGetBySelector("fillCCDetailsDiv", ApplicationSettings.TimeOut.Safe);
            }while (fillCcDetailsDiv == null || !fillCcDetailsDiv.Displayed) ;
        }

        internal void PayNow(PaymentInfo paymentInfo)
        {
            SetPaymentMode(paymentInfo.PaymentMode);
            SetAddress(paymentInfo.BillingAddress);

            WaitAndGetBySelector("checkTerms", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("paynow", ApplicationSettings.TimeOut.Fast).Click(); 
        }

        internal void BookNow()
        {
            WaitAndGetBySelector("checkTerms", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("booknow", ApplicationSettings.TimeOut.Fast).Click(); 
        }

        #endregion

        public void CheckPaymentStatus()
        {
            try
            {
                do
                {
                    var paymentStatus = WaitAndGetBySelector("alertSuccess", ApplicationSettings.TimeOut.Safe);
                    if (paymentStatus != null && paymentStatus.Displayed)
                        break;
                    CheckAndThrowErrors();
                } while (true);
            }
            catch (Exception exception)
            {
                LogManager.GetInstance().LogInformation("Payment Failed"); 
                throw;
            }
        }
    }
}
