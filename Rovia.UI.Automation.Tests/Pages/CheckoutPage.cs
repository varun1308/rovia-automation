using System;
using System.Configuration;
using System.Threading;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Model;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Pages
{
    public class CheckoutPage : UIPage
    {
        internal bool IsPayNowSuccess_CreditCard()
        {
            var fillCcDetailsDiv = WaitAndGetBySelector("fillCCDetailsDiv", ApplicationSettings.TimeOut.Safe);
            while (fillCcDetailsDiv != null && !fillCcDetailsDiv.Displayed)
                break;
            
            WaitAndGetBySelector("checkTerms", ApplicationSettings.TimeOut.Safe).Click();
            WaitAndGetBySelector("paynow", ApplicationSettings.TimeOut.Fast).Click();

            var preLoadingDiv = WaitAndGetBySelector("preloading", ApplicationSettings.TimeOut.Slow);
            while (preLoadingDiv != null && preLoadingDiv.Displayed)
            {
                var bfcPageDiv = WaitAndGetBySelector("bfcPageDiv", ApplicationSettings.TimeOut.Safe);
                var divPayNowfailed = WaitAndGetBySelector("divPayNowfailed", ApplicationSettings.TimeOut.SuperFast);
                if (divPayNowfailed != null && divPayNowfailed.Displayed)
                    return false;
                else if (bfcPageDiv != null && bfcPageDiv.Displayed)
                    return true;
            }
            return false;
        }

        internal bool MakePayment_CreditCard(PaymentFields paymentFields)
        {
            var fillCcDetailsDiv = WaitAndGetBySelector("fillCCDetailsDiv", ApplicationSettings.TimeOut.Safe);
            if (fillCcDetailsDiv != null && fillCcDetailsDiv.Displayed)
            {
                Thread.Sleep(5000);

                var divPaymentBox = WaitAndGetBySelector("divPaymentBox", ApplicationSettings.TimeOut.Safe);
                if (divPaymentBox != null && divPaymentBox.Displayed)
                {
                    ExecuteJavascript("$('#NameOnCard').val('" + paymentFields.CreditCard.NameOnCard + "')");
                    ExecuteJavascript("$('#CardNumber').val('" + paymentFields.CreditCard.CardNumber + "')");
                    ExecuteJavascript("$('#SecurityCode').val('" + paymentFields.CreditCard.SecurityCode + "')");
                    ExecuteJavascript("$('#ExpirationMonth').val('" + paymentFields.CreditCard.ExpirationMonth + "')");
                    ExecuteJavascript("$('#ExpirationYear').val('" + paymentFields.CreditCard.ExpirationYear + "')");
                    ExecuteJavascript("$('#Address1').val('" + paymentFields.Address.Address1 + "')");
                    ExecuteJavascript("$('#City').val('" + paymentFields.Address.City + "')");
                    ExecuteJavascript("$('#Country').val('" + paymentFields.Address.Country +
                                      "');$('#Country').change();");

                    var SpinningDiv = WaitAndGetBySelector("SpinningDiv", ApplicationSettings.TimeOut.Slow);
                    while (SpinningDiv != null && SpinningDiv.Displayed)
                        Thread.Sleep(200);

                    ExecuteJavascript("$('#PostalCode').val('" + paymentFields.Address.PostalCode + "')");
                    ExecuteJavascript("$('#Provinces').val('" + paymentFields.Address.Provinces + "')");
                    ExecuteJavascript("$('#PhoneNumberArea').val('" + paymentFields.PhoneNumber.PhoneNumberArea + "')");
                    ExecuteJavascript("$('#PhoneNumberExchange').val('" + paymentFields.PhoneNumber.PhoneNumberExchange +
                                      "')");
                    ExecuteJavascript("$('#PhoneNumberDigits').val('" + paymentFields.PhoneNumber.PhoneNumberDigits +
                                      "')");
                    ExecuteJavascript("$('#EmailAddress').val('" + paymentFields.EmailAddress + "')");
                    Thread.Sleep(1000);
                    WaitAndGetBySelector("SubmitPayment", ApplicationSettings.TimeOut.Fast).Click();
                    
                    while (SpinningDiv != null && SpinningDiv.Displayed)
                    {
                        Thread.Sleep(2000);
                        var confirmationDiv = WaitAndGetBySelector("ConfirmationDiv", ApplicationSettings.TimeOut.Slow);
                        if (confirmationDiv != null && confirmationDiv.Displayed)
                            return true;
                    }
                }
            }
            return false;
        }

        internal bool IsPayNowSuccess_RB(PaymentFields paymentFields)
        {
            var fillCcDetailsDiv = WaitAndGetBySelector("fillCCDetailsDiv", ApplicationSettings.TimeOut.Safe);
            while (fillCcDetailsDiv != null && !fillCcDetailsDiv.Displayed)
                break;

            var roviaBucksCheck = WaitAndGetBySelector("roviaBucksCheck", ApplicationSettings.TimeOut.Safe);
            if (roviaBucksCheck != null && roviaBucksCheck.Displayed)
            {
                WaitAndGetBySelector("roviaBucksCheck",ApplicationSettings.TimeOut.Fast).Click();
                WaitAndGetBySelector("typeRB", ApplicationSettings.TimeOut.Fast).SelectFromDropDown("Standard");

                WaitAndGetBySelector("address1", ApplicationSettings.TimeOut.Fast).SendKeys(paymentFields.Address.Address1);
                WaitAndGetBySelector("address2", ApplicationSettings.TimeOut.Fast).SendKeys("");


                ExecuteJavascript("$('#drpCountry').val('" + paymentFields.Address.Country +
                                  "');$('#drpCountry').change();");
                
                Thread.Sleep(2000);

                var txtstate = WaitAndGetBySelector("txtstate", ApplicationSettings.TimeOut.Slow);
                if (txtstate != null && txtstate.Displayed)
                    WaitAndGetBySelector("txtstate", ApplicationSettings.TimeOut.Safe).SendKeys(paymentFields.Address.Provinces);
                else
                    ExecuteJavascript("$('#drpState').val('" + paymentFields.Address.Provinces + "')");
                
                WaitAndGetBySelector("city", ApplicationSettings.TimeOut.Fast).SendKeys(paymentFields.Address.City);

                WaitAndGetBySelector("zipcode", ApplicationSettings.TimeOut.Fast).SendKeys(paymentFields.Address.PostalCode);

                WaitAndGetBySelector("phnArea", ApplicationSettings.TimeOut.Fast).SendKeys(paymentFields.PhoneNumber.PhoneNumberArea);
                WaitAndGetBySelector("phnExchange", ApplicationSettings.TimeOut.Fast).SendKeys(paymentFields.PhoneNumber.PhoneNumberExchange);
                WaitAndGetBySelector("phnDigits", ApplicationSettings.TimeOut.Fast).SendKeys(paymentFields.PhoneNumber.PhoneNumberDigits);
                
                WaitAndGetBySelector("checkTerms", ApplicationSettings.TimeOut.Fast).Click();
                WaitAndGetBySelector("paynow", ApplicationSettings.TimeOut.Fast).Click();

                var preLoadingDiv = WaitAndGetBySelector("preloading", ApplicationSettings.TimeOut.Slow);
                while (preLoadingDiv != null && preLoadingDiv.Displayed)
                {
                    Thread.Sleep(2000);

                    var confirmationDiv = WaitAndGetBySelector("ConfirmationDiv", ApplicationSettings.TimeOut.Safe);
                    
                    if (confirmationDiv != null && confirmationDiv.Displayed)
                        return true;
                }

                var divPayNowfailed = WaitAndGetBySelector("divPayNowfailed", ApplicationSettings.TimeOut.Slow);
                if (divPayNowfailed != null && divPayNowfailed.Displayed)
                    return false;
            }
            return false;
        }

        internal void WaitForLoad()
        {
            throw new NotImplementedException();
        }

        public void PayNow(PaymentInfo paymentInfo)
        {
            try
            {
                WaitAndGetBySelector("roviaBucksCheck", ApplicationSettings.TimeOut.Fast).Click();
                SetAddress(paymentInfo.Address);

                WaitAndGetBySelector("checkTerms", ApplicationSettings.TimeOut.Fast).Click();
                WaitAndGetBySelector("paynow", ApplicationSettings.TimeOut.Fast).Click();
            }
            catch (Exception exception)
            {
                throw new Exception("Roviabucks Payment Failed");
            }
        }

        internal void ConfirmPayment()
        {
            try
            {
                var preLoadingDiv = WaitAndGetBySelector("preloading", ApplicationSettings.TimeOut.Slow);
                while (preLoadingDiv != null && preLoadingDiv.Displayed)
                {
                    Thread.Sleep(2000);

                    var confirmationDiv = WaitAndGetBySelector("ConfirmationDiv", ApplicationSettings.TimeOut.Safe);

                    if (confirmationDiv != null && confirmationDiv.Displayed)
                        break;
                }

                var divPayNowfailed = WaitAndGetBySelector("divPayNowfailed", ApplicationSettings.TimeOut.Slow);
                if (divPayNowfailed != null && divPayNowfailed.Displayed)
                    throw new Exception();
            }
            catch (Exception exception)
            {
                throw new Exception("Payment Confirmation Failed",exception);
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
            catch (Exception exception )
            {
                throw new Exception("Error While Entering billing Address",exception);
            }
        }

        internal void PayNow(PaymentMode paymentMode)
        {
            //todo implement checking creditcard checkbox
        }
    }
}
