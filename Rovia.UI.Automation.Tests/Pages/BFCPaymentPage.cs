
using System;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages
{
    public class BFCPaymentPage : UIPage
    {
        internal void WaitForLoad()
        {
            IUIWebElement nameOnCard;
            do
            {
                nameOnCard = WaitAndGetBySelector("lblNameOnCard", ApplicationSettings.TimeOut.Fast);
            } while (nameOnCard == null || !nameOnCard.Displayed);
        }

        public void PayNow(PaymentInfo paymentInfo)
        {
            try
            {
                SetCreditCardInfo(paymentInfo.CreditCardInfo);
                SetBillingAddress(paymentInfo.BillingAddress);
                WaitAndGetBySelector("payNow", ApplicationSettings.TimeOut.Fast).Click();
                //WaitForPayment();
                CheckErrors();
            }
            catch (Exception exception)
            {
                throw new Exception("payNow failed", exception);
            }
        }

        private void WaitForPayment()
        {
            try
            {
                IUIWebElement divSpinner;
                do
                {
                    Thread.Sleep(1000);
                    if (WaitAndGetBySelector("divSpinner", ApplicationSettings.TimeOut.Fast) == null)
                        break;
                } while (true);
            }
            catch (Exception exception)
            {
                
                throw;
            }
        }

        private void CheckErrors()
        {
            var errors = WaitAndGetBySelector("divError", ApplicationSettings.TimeOut.Slow);
            if (errors.Displayed)
                throw new Exception(errors.Text);
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
	        catch (NullReferenceException exception)
	        {
		        throw new Exception("Exception while setting Address Info", exception);
	        }
        }

        private void SetCreditCardInfo(CreditCardInfo creditCardInfo)
        {
            try
            {
                Thread.Sleep(1000);
                WaitAndGetBySelector("inpNameOnCard", ApplicationSettings.TimeOut.Fast).SendKeys(creditCardInfo.NameOnCard);
                WaitAndGetBySelector("inpCardNumber", ApplicationSettings.TimeOut.Fast).SendKeys(creditCardInfo.CardNo);
                WaitAndGetBySelector("inpSecurityCode", ApplicationSettings.TimeOut.Fast).SendKeys(creditCardInfo.Cvv);
                WaitAndGetBySelector("selectExpirationMonth", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(creditCardInfo.ExpiryDate.ToString("MMMM"));
                WaitAndGetBySelector("selectExpirationYear", ApplicationSettings.TimeOut.Fast).SelectFromDropDown(creditCardInfo.ExpiryDate.Year.ToString());
                WaitAndGetBySelector("inpEmailAddresse", ApplicationSettings.TimeOut.Fast).SendKeys(creditCardInfo.CardHolderEmailId);
            }
            catch (NullReferenceException exception)
            {
                throw new Exception("Exception while setting CreditCard Info", exception);
            }
        }
    }
}
