using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Utility;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Pages
{
   public class PassengerInfoPage : UIPage
    {

       internal void ConfirmPassengers()
       {
           Thread.Sleep(3000);
           WaitAndGetBySelector("ConfirmPxButton", ApplicationSettings.TimeOut.Slow).Click();
           //var SpinningDiv = WaitAndGetBySelector("SpinningDiv", ApplicationSettings.TimeOut.Slow);
           //if (SpinningDiv != null && SpinningDiv.Displayed)
       }

       public void WaitForPageLoad()
       {
           try
           {
               while (WaitAndGetBySelector("SpinningDiv", ApplicationSettings.TimeOut.Slow).Displayed);
           }
           catch (Exception exception)
           {
               throw new Exception("PassengerDetailsPage Load Failed", exception);
           }
       }

       public void SubmitPassengerDetails(PassengerDetails passengerDetails)
       {
           try
           {
               WaitAndGetBySelector("country", ApplicationSettings.TimeOut.Slow).SelectFromDropDown(passengerDetails.Country);
               if (passengerDetails.IsInsuranceRequired)
                   WaitAndGetBySelector("selectInsurance", ApplicationSettings.TimeOut.Slow).Click();
               EnterPassengerDetails(passengerDetails);
               WaitAndGetBySelector("Submitbutton", ApplicationSettings.TimeOut.Fast).Click();
           }
           catch (NullReferenceException exception)
           {
               throw new Exception("Passenger detail Elements not Loaded properly",exception);
           }
       }

       private void EnterPassengerDetails(PassengerDetails passengerDetails)
       {
           ExecuteJavascript("$('input.span7.jsDob').prop('disabled',false);");
           var inputForm = GetInputForm();
           (new List<List<IUIWebElement>>(inputForm.Values.Take(6))).ForEach(x => x.ForEach(y => y.Clear()));
           var i = 0;
           passengerDetails.Passengers.ForEach(x =>
           {
               inputForm["fNames"][i].SendKeys(x.FirstName);
               inputForm["mNames"][i].SendKeys(x.MiddleName);
               inputForm["lNames"][i].SendKeys(x.LastName);
               inputForm["eMail"][i].SendKeys(x.Emailid);
               inputForm["dob"][i].SendKeys(DateTime.Now.AddYears(-1 * x.Age).AddDays(-6).ToString("MM/dd/yyyy"));
               inputForm["gender"][i].SelectFromDropDown(x.Gender);
               inputForm["vEmail"][i].SendKeys(x.Emailid);
               ++i;
           });
       }

       private Dictionary<string, List<IUIWebElement>> GetInputForm()
       {
           return new Dictionary<string, List<IUIWebElement>>()
               {
                   {"fNames", GetUIElements("fNames")},
                   {"mNames", GetUIElements("mNames")},
                   {"lNames", GetUIElements("lNames")},
                   {"eMail", GetUIElements("eMail")},
                   {"vEmail", GetUIElements("vEmail")},
                   {"dob", GetUIElements("dob")},
                   {"gender", GetUIElements("gender")},
               };
       }
    }
}
