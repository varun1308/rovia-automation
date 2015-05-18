namespace Rovia.UI.Automation.Tests.Pages
{
    using System;
    using System.Threading;
    using AppacitiveAutomationFramework;
    using OpenQA.Selenium;
    using Exceptions;
    using Logger;
    using ScenarioObjects;
    using Configuration;

    /// <summary>
    /// This class holds all the fields and methods for BFC Payment page
    /// </summary>
    public class BFCPaymentPage : UIPage
    {
        #region Private Members

        private void WaitForPayment()
        {
            try
            {
                do
                {
                    Thread.Sleep(1000);
                    if (WaitAndGetBySelector("divSpinner", ApplicationSettings.TimeOut.Fast) == null)
                        break;
                } while (true);
            }
            catch (StaleElementReferenceException)
            {
                LogManager.GetInstance().LogInformation("StaleElementReferenceException caught and suppressed");
            }
        }

        private void CheckErrors()
        {
            var errors = WaitAndGetBySelector("divError", ApplicationSettings.TimeOut.Slow);
            if (errors!=null &&errors.Displayed)
                throw new Alert(errors.Text);
        }

        private void SetBillingAddress(BillingAddress address)
        {
            try
            {
                WaitAndGetBySelector("inpAddress1", ApplicationSettings.TimeOut.Fast).SendKeys(address.Line1);
                WaitAndGetBySelector("inpAddress2", ApplicationSettings.TimeOut.Fast).SendKeys(address.Line2);
                WaitAndGetBySelector("inpCity", ApplicationSettings.TimeOut.Fast).SendKeys(address.City);
                WaitAndGetBySelector("selectCountry", ApplicationSettings.TimeOut.Safe).SelectFromDropDown(address.Country);
                Thread.Sleep(2000);
                WaitAndGetBySelector("selectProvinces", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(address.State);
                WaitAndGetBySelector("inpPhoneNumberArea", ApplicationSettings.TimeOut.Fast).SendKeys(address.PhoneNumber.Substring(0, 3));
                WaitAndGetBySelector("inpPhoneNumberExchange", ApplicationSettings.TimeOut.Fast).SendKeys(address.PhoneNumber.Substring(3, 3));
                WaitAndGetBySelector("inpPhoneNumberDigits", ApplicationSettings.TimeOut.Fast).SendKeys(address.PhoneNumber.Substring(6));
                WaitAndGetBySelector("inpPostalCode", ApplicationSettings.TimeOut.Fast).SendKeys(address.ZipCode);
            }
            catch (Exception)
            {
                LogManager.GetInstance().LogInformation("Set Billing Address Failed");
                throw;
            }
        }

        private void SetCreditCardInfo(CreditCardInfo creditCardInfo)
        {
            try
            {
                Thread.Sleep(1000);
                WaitAndGetBySelector("inpNameOnCard", ApplicationSettings.TimeOut.Fast).SendKeys(creditCardInfo.NameOnCard);
                WaitAndGetBySelector("selectCardType",ApplicationSettings.TimeOut.Fast).SelectFromDropDown("Visa"); //This is hard coded as for now BFC accepts Visa Card only on PreQA/QA environments!!
                WaitAndGetBySelector("inpCardNumber", ApplicationSettings.TimeOut.Fast).SendKeys(creditCardInfo.CardNo);
                WaitAndGetBySelector("inpSecurityCode", ApplicationSettings.TimeOut.Fast).SendKeys(creditCardInfo.Cvv);
                WaitAndGetBySelector("selectExpirationMonth", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(creditCardInfo.ExpiryDate.ToString("MMMM"));
                WaitAndGetBySelector("selectExpirationYear", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(creditCardInfo.ExpiryDate.Year.ToString());
                WaitAndGetBySelector("inpEmailAddresse", ApplicationSettings.TimeOut.Fast).SendKeys(creditCardInfo.CardHolderEmailId);
            }
            catch (Exception)
            {
                LogManager.GetInstance().LogInformation("Setting CrediCard Info Failed");
                throw;
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Wait for Bfc Payment page to load
        /// </summary>
        public void WaitForLoad()
        {
            IUIWebElement nameOnCard;
            do
            {
                nameOnCard = WaitAndGetBySelector("lblNameOnCard", ApplicationSettings.TimeOut.Fast);
            } while (nameOnCard == null || !nameOnCard.Displayed);
        }

        /// <summary>
        /// Enter and submit all the required fields for CC payment
        /// </summary>
        /// <param name="paymentInfo"></param>
        public void PayNow(PaymentInfo paymentInfo)
        {
            SetCreditCardInfo(paymentInfo.CreditCardInfo);
            SetBillingAddress(paymentInfo.BillingAddress);
            WaitAndGetBySelector("payNow", ApplicationSettings.TimeOut.Fast).Click();
            WaitForPayment();
            CheckErrors();
        }

        #endregion

    }
}
