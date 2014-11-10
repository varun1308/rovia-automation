using System;
using System.Threading;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Model;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Pages
{
    public class CheckoutPage : UIPage
    {
        internal bool IsPayNowSuccess()
        {
                var fillCcDetailsDiv = WaitAndGetBySelector("fillCCDetailsDiv", ApplicationSettings.TimeOut.Slow);
                while (fillCcDetailsDiv != null && !fillCcDetailsDiv.Displayed)
                    break;
                    ExecuteJavascript("$('#chkTNC').prop('checked',true)");
                    ExecuteJavascript("$('.jsBtnPayNow.btn.btn-warning').click()");

                    //Need to handle failure case as of now assumed success case
                    var preLoadingDiv = WaitAndGetBySelector("preloading", ApplicationSettings.TimeOut.Slow);
                    while (preLoadingDiv != null && preLoadingDiv.Displayed)
                    {
                        Thread.Sleep(2000);
                        var divPayNowfailed = WaitAndGetBySelector("divPayNowfailed", ApplicationSettings.TimeOut.Slow);
                        if (divPayNowfailed != null && divPayNowfailed.Displayed)
                            return false;
                        else
                            return true;
                    }
                
            return false;
        }

        internal bool MakePayment(PaymentFields paymentFields)
        {
            var fillCcDetailsDiv = WaitAndGetBySelector("fillCCDetailsDiv", ApplicationSettings.TimeOut.Slow);
            if (fillCcDetailsDiv != null && fillCcDetailsDiv.Displayed)
            {
                Thread.Sleep(10000);
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

                Thread.Sleep(8000);
                var ConfirmationDiv = WaitAndGetBySelector("ConfirmationDiv", ApplicationSettings.TimeOut.Slow);
                if (ConfirmationDiv != null && ConfirmationDiv.Displayed)
                    return true;
            }
            return false;
        }

    }
}
