using System.Globalization;

namespace Rovia.UI.Automation.Tests.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using AppacitiveAutomationFramework;
    using Exceptions;
    using Logger;
    using Configuration;
    using Utility;
    using ScenarioObjects;
    using Validators;

    /// <summary>
    /// This class contains all the fields and methods for passenger info page
    /// </summary>
    public class PassengerInfoPage : UIPage
    {
        #region Private Members

        private static List<Passenger> _passengers;

        private bool IsInputFormVisible()
        {
            try
            {
                return WaitAndGetBySelector("divPassengerDetailInput", ApplicationSettings.TimeOut.Slow).Displayed;
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
            //ExecuteJavascript("$('input.span7.jsDob').prop('disabled',false);");
            var inputForm = GetInputForm();
            (new List<List<IUIWebElement>>(inputForm.Values.Take(6))).ForEach(x => x.ForEach(y => y.Clear()));
            var i = 0;
            _passengers.ForEach(x =>
            {
                inputForm["fNames"][i].SendKeys(x.FirstName);
                inputForm["mNames"][i].SendKeys(x.MiddleName);
                inputForm["lNames"][i].SendKeys(x.LastName);

                if (inputForm["dob"].Count == 1)
                {
                    ExecuteJavascript("$('input.span7.jsDob').prop('disabled',false);");
                    inputForm["dob"][i].SendKeys(x.BirthDate.Replace('-', '/'));
                }
                inputForm["eMail"][i].Click();
                inputForm["eMail"][i].SendKeys(x.Emailid);
                inputForm["vEmail"][i].SendKeys(x.Emailid);
                
                inputForm["gender"][i].SelectFromDropDown(x.Gender);
                ++i;
            });
        }

        private void VerifyPassengerDetails()
        {
            var paxConfDetails = GetUIElements("paxConfDiv").Select(x => x.Text.Split(new[] { '\r', '\n' }).ToList()).ToList();
            paxConfDetails.ForEach(x => x.RemoveAll(string.IsNullOrEmpty));
            if (TestHelper.TripProductType != TripProductType.Car )
            {
                VerifyPaxDetails(paxConfDetails.Select(GetPassenger));
            }
        }

        private static void VerifyPaxDetails(IEnumerable<Passenger> passengers)
        {
            if (_passengers.Zip(passengers, (x, y) => x.Equals(y)).Any(x => x.Equals(false)))
                throw new ValidationException("Passenger Details");
        }

        private static Passenger GetPassenger(List<string> passengerElements)
        {
            var today = DateTime.Today;
            var bday = DateTime.ParseExact(passengerElements[passengerElements.IndexOf("BIRTHDATE") + 1], "mm/dd/yyyy", new CultureInfo("en-US"), DateTimeStyles.None);

            var age = today.Year - bday.Year;
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

        private void WaitForConfirmationPageLoad()
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

        #endregion

        #region Public Properties

        public TripProductHolder TripProductHolder { get; set; }

        #endregion

        #region Public Members

        /// <summary>
        /// Wait for passenger info page to load
        /// </summary>
        /// <param name="confirmAlert">Do action if rovia award popup appears</param>
        public void WaitForPageLoad(Action confirmAlert)
        {
            while (WaitAndGetBySelector("SpinningDiv", ApplicationSettings.TimeOut.Fast) == null)
                confirmAlert();

            var startCount = Environment.TickCount;
            while (WaitAndGetBySelector("SpinningDiv", ApplicationSettings.TimeOut.Fast).Displayed)
            {
                if (Environment.TickCount - startCount > 120000)
                    throw new PageLoadFailed("passengerInfoPage", new TimeoutException());
            }
            if (WaitAndGetBySelector("divPassengerHolder", ApplicationSettings.TimeOut.Safe).Displayed == false)
                throw new PageLoadFailed("PassengerInfoPage");
        }

        /// <summary>
        /// Enter passenger details and submit to confirm passenger info page
        /// </summary>
        /// <param name="passengerDetails"></param>
        public void SubmitPassengerDetails(PassengerDetails passengerDetails)
        {
            Thread.Sleep(1500);
            _passengers = passengerDetails.Passengers;
            if (TestHelper.TripProductType == TripProductType.Air || TestHelper.TripProductType == TripProductType.Hotel)
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

        /// <summary>
        /// Edit passenger information
        /// </summary>
        public void EditPassengerInfo()
        {
            WaitAndGetBySelector("lnkEditPassengerInfo", ApplicationSettings.TimeOut.Slow).Click();
            if (IsInputFormVisible() == false)
                throw new UIElementNullOrNotVisible("PassengerDetails InputForm");
            WaitAndGetBySelector("Submitbutton", ApplicationSettings.TimeOut.Fast).Click();
            WaitForConfirmationPageLoad();
        }

        /// <summary>
        /// Confirm the passenger details provided on passenger info page
        /// </summary>
        public void ConfirmPassengers()
        {
            VerifyPassengerDetails();
            WaitAndGetBySelector("ConfirmPxButton", ApplicationSettings.TimeOut.Slow).Click();
        }
       
        /// <summary>
        /// Validate trip  details on passenger info page
        /// </summary>
        /// <param name="selectedItinerary">added itinerary to cart</param>
        public void ValidateTripDetails(Results selectedItinerary)
        {
            TripProductHolder.GetTripProducts().ForEach(x => this.ValidateTripProduct(x, selectedItinerary));
        }

        /// <summary>
        /// Waits for unwanted alerts on browser and confirm them
        /// </summary>
        /// <returns></returns>
        public bool IsLeavePopupVisible()
        {
            try
            {
                var divDontLeavePopup = WaitAndGetBySelector("dontLeavePopup", ApplicationSettings.TimeOut.Slow);
                return divDontLeavePopup != null && divDontLeavePopup.Displayed;
            }
            catch (OpenQA.Selenium.StaleElementReferenceException)
            {
                LogManager.GetInstance().LogWarning("Rovia Award Popup - Stale element reference caught and suppressed.");
            }
            return false;
        }
        
        #endregion

    }
}
