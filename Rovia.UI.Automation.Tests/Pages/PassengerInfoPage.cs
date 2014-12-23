using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Utility;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Pages
{
   public class PassengerInfoPage : UIPage
   {
       #region Private Members

       private static List<Passenger> _passengers;

       private bool IsInputFormVisible()
       {
           try
           {
               return WaitAndGetBySelector("divPassengerDetailInput", ApplicationSettings.TimeOut.Slow).Displayed ;
           }
           catch (Exception)
           {
               LogManager.GetInstance().LogDebug("passenger details InputForm not visible"); 
               return false;
           }
       }

       private bool IsPassengerInfoConfirmationPageVisible()
       {
           try
           {
               return WaitAndGetBySelector("divConfirmPassengerDetails", ApplicationSettings.TimeOut.Slow).Displayed;
           }
           catch (Exception)
           {
               LogManager.GetInstance().LogDebug("passenger details Confirmation page not visible"); 
               return false;
           }
       }

       private void EnterPassengerDetails()
       {
           ExecuteJavascript("$('input.span7.jsDob').prop('disabled',false);");
           var inputForm = GetInputForm();
           (new List<List<IUIWebElement>>(inputForm.Values.Take(6))).ForEach(x => x.ForEach(y => y.Clear()));
           var i = 0;
           _passengers.ForEach(x =>
           {
               inputForm["fNames"][i].SendKeys(x.FirstName);
               inputForm["mNames"][i].SendKeys(x.MiddleName);
               inputForm["lNames"][i].SendKeys(x.LastName);
               inputForm["eMail"][i].SendKeys(x.Emailid);
               inputForm["dob"][i].SendKeys(x.BirthDate);
               inputForm["gender"][i].SelectFromDropDown(x.Gender);
               inputForm["vEmail"][i].SendKeys(x.Emailid);
               ++i;
           });
       }

       private void VerifyPassengerDetails()
       {
           var paxConfDetails = GetUIElements("paxConfDiv").Select(x => x.Text.Split(new []{'\r','\n'}).ToList()).ToList();
           paxConfDetails.ForEach(x=>x.RemoveAll(string.IsNullOrEmpty));
           VerifyPaxDetails(paxConfDetails.Select(GetPassenger));
           
       }

       private static void VerifyPaxDetails(IEnumerable<Passenger> passengers)
       {
           if (_passengers.Zip(passengers, (x, y) => x.Equals(y)).Any(x=>x.Equals(false)))
               throw new ValidationException("Passenger Details");
       }

       private static Passenger GetPassenger(List<string> passengerElements)
       {
           var today = DateTime.Today;
           var bday = DateTime.Parse(passengerElements[passengerElements.IndexOf("BIRTHDATE") + 1]);
           var age = today.Year-bday.Year;
           if (bday > today.AddYears(-age)) age--;
           if (age <= 2)
               return new Infant(passengerElements);
           if (age < 18)
               return new Child(passengerElements);
           return new Adult(passengerElements);
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

       #endregion

       internal void EditPassengerInfo()
       {
           WaitAndGetBySelector("lnkEditPassengerInfo", ApplicationSettings.TimeOut.Slow).Click();
           if (IsInputFormVisible() == false)
               throw new UIElementNullOrNotVisible("PassengerDetails InputForm");
           WaitAndGetBySelector("Submitbutton", ApplicationSettings.TimeOut.Fast).Click();
           WaitForConfirmationPageLoad();
       }

       internal void ConfirmPassengers()
       {
           VerifyPassengerDetails();
           WaitAndGetBySelector("ConfirmPxButton", ApplicationSettings.TimeOut.Slow).Click();
       }

       public void WaitForPageLoad()
       {
           while (WaitAndGetBySelector("SpinningDiv", 60).Displayed) ;
           if (WaitAndGetBySelector("divPassengerHolder", ApplicationSettings.TimeOut.Safe).Displayed == false)
               throw new PageLoadFailed("PassengerInfoPage");
       }

       public void WaitForConfirmationPageLoad()
       {
           try
           {
               while (IsPassengerInfoConfirmationPageVisible() == false)
               {
                   var alertError = WaitAndGetBySelector("alertError", ApplicationSettings.TimeOut.Fast);
                   if (alertError != null && alertError.Displayed)
                   {
                       throw new Alert(alertError.Text);

                   }
               }

           }
           catch (Exception exception)
           {
               throw new PageLoadFailed("PassengerCOnfirmationPage", exception);
           }
    }

       public void SubmitPassengerDetails(PassengerDetails passengerDetails)
       {
           Thread.Sleep(1500);
           _passengers = passengerDetails.Passengers;
           if (!TestHelper.TripProductType.Equals(TripProductType.Car))
           {
               WaitAndGetBySelector("country", ApplicationSettings.TimeOut.Slow).SelectFromDropDown(
                   passengerDetails.Country);
               if (passengerDetails.IsInsuranceRequired)
                   WaitAndGetBySelector("selectInsurance", ApplicationSettings.TimeOut.Slow).Click();
           }
           EnterPassengerDetails();
           WaitAndGetBySelector("Submitbutton", ApplicationSettings.TimeOut.Slow).Click();
           WaitForConfirmationPageLoad();
       }

    }
}
