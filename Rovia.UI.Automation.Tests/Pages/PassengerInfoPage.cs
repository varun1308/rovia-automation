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
   public class PassengerInfoPage : UIPage
    {
       internal bool SubmitPassengerDetails(PassengerDetails passenger)
       {
           var spinningDiv = WaitAndGetBySelector("SpinningDiv", ApplicationSettings.TimeOut.Slow);
           while (spinningDiv != null && spinningDiv.Displayed);

           Thread.Sleep(500);
           var divCartLoaded= WaitAndGetBySelector("divCartLoaded", ApplicationSettings.TimeOut.Slow);
           if (divCartLoaded != null && divCartLoaded.Displayed)
           {
               WaitAndGetBySelector("countryInsurance", ApplicationSettings.TimeOut.Slow).SelectFromDropDown(passenger.InsuranceData.Country);
               ExecuteJavascript("$('input.span7.jsDob').prop('disabled', false);");
               ExecuteJavascript("$('input.span12.jsFName').val('"+passenger.FirstName+"')");
               ExecuteJavascript("$('input.span12.jsLName').val('"+passenger.LastName+"')");
               ExecuteJavascript("$('input.span7.jsDob').val('"+passenger.DOB+"')");
               ExecuteJavascript("$('input.span11.jsEmail').val('"+passenger.Emailid+"')");
               ExecuteJavascript("$('input.span11.jsVEmail').val('"+passenger.Emailid+"')");
               ExecuteJavascript("$('select.span10.jsGender').val('"+passenger.Gender+"')");
               Thread.Sleep(5000);
               WaitAndGetBySelector("Submitbutton", ApplicationSettings.TimeOut.Fast).Click();
               ConfirmPassengers();
               return true;
           }
           return false;
       }

       internal void ConfirmPassengers()
       {
           Thread.Sleep(3000);
           WaitAndGetBySelector("ConfirmPxButton", ApplicationSettings.TimeOut.Slow).Click();
           //var SpinningDiv = WaitAndGetBySelector("SpinningDiv", ApplicationSettings.TimeOut.Slow);
           //if (SpinningDiv != null && SpinningDiv.Displayed)
       }
    }
}
