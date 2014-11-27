using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Utility;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Pages
{
   public class PassengerInfoPage : UIPage
   {
       private static PassengerDetails _passengerDetails;
       internal void EditPassengerInfo()
       {
           try
           {
               WaitAndGetBySelector("lnkEditPassengerInfo", ApplicationSettings.TimeOut.Slow).Click();
               if(IsInputFormVisible()==false)
                   throw new Exception("InputForm failed To load");
               WaitAndGetBySelector("Submitbutton", ApplicationSettings.TimeOut.Fast).Click();
               WaitForConfirmationPageLoad();
           }
           catch (NullReferenceException exception)
           {
               throw new Exception("Edit PassengerInfo Button not visible", exception);
           }
       }

       internal void ConfirmPassengers()
       {
           try
           {
               //todo Confirm Passenger Details
               WaitAndGetBySelector("ConfirmPxButton", ApplicationSettings.TimeOut.Slow).Click();
           }
           catch (NullReferenceException exception)
           {
               throw new Exception("Confirmation Button not visible", exception);
           }
       }

       public void WaitForPageLoad()
       {
               while (WaitAndGetBySelector("SpinningDiv", 60).Displayed) ;
               if (WaitAndGetBySelector("divPassengerHolder", ApplicationSettings.TimeOut.Safe).Displayed == false)
                   throw new PageNotFoundException("passengerinfo");
       }
       public void WaitForConfirmationPageLoad()
       {
           try
           {
               while (IsPassengerInfoConfirmationPageVisible() == false)
               {
                   var alertError = WaitAndGetBySelector("alertError", ApplicationSettings.TimeOut.Fast);
                   if (alertError!=null && alertError.Displayed)
                   {
                       throw new Exception(alertError.Text);
                       
                   }
               }

           }
           catch (Exception exception)
           {
               throw new Exception("PassengerCOnfirmationPage Load Failed", exception);
           }
       }
       private bool IsInputFormVisible()
       {
           try
           {
               return WaitAndGetBySelector("divPassengerDetailInput", ApplicationSettings.TimeOut.Slow).Displayed ;

           }
           catch (Exception exception)
           {
               throw new Exception("PassengerDetailsPage InputForm Load Failed", exception);
           }
       }

       private bool IsPassengerInfoConfirmationPageVisible()
       {
           try
           {
               return WaitAndGetBySelector("divConfirmPassengerDetails", ApplicationSettings.TimeOut.Slow).Displayed;
           }
           catch (Exception exception)
           {
               return false;
           }
       }

       public void SubmitPassengerDetails(PassengerDetails passengerDetails)
       {
           try
           {
               Thread.Sleep(1500);
               _passengerDetails = passengerDetails;
               WaitAndGetBySelector("country", ApplicationSettings.TimeOut.Slow).SelectFromDropDown(passengerDetails.Country);
               if (passengerDetails.IsInsuranceRequired)
                   WaitAndGetBySelector("selectInsurance", ApplicationSettings.TimeOut.Slow).Click();
               EnterPassengerDetails();
               WaitAndGetBySelector("Submitbutton", ApplicationSettings.TimeOut.Slow).Click();
               WaitForConfirmationPageLoad();
           }
           catch (NullReferenceException exception)
           {
               throw new Exception("Passenger detail Elements not Loaded properly",exception);
           }
       }

       private void EnterPassengerDetails()
       {
           ExecuteJavascript("$('input.span7.jsDob').prop('disabled',false);");
           var inputForm = GetInputForm();
           (new List<List<IUIWebElement>>(inputForm.Values.Take(6))).ForEach(x => x.ForEach(y => y.Clear()));
           var i = 0;
           _passengerDetails.Passengers.ForEach(x =>
           {
               inputForm["fNames"][i].SendKeys(x.FirstName);
               inputForm["mNames"][i].SendKeys(x.MiddleName);
               inputForm["lNames"][i].SendKeys(x.LastName);
               inputForm["eMail"][i].SendKeys(x.Emailid);
               inputForm["dob"][i].SendKeys(DateTime.Now.AddYears(-1 * x.Age).AddMonths(3).ToString("MM/dd/yyyy"));
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
