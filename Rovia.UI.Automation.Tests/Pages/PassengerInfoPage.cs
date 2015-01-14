using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects.Activity;
using Rovia.UI.Automation.ScenarioObjects.Hotel;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Utility;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Validators;
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
            var paxConfDetails = GetUIElements("paxConfDiv").Select(x => x.Text.Split(new[] { '\r', '\n' }).ToList()).ToList();
            paxConfDetails.ForEach(x => x.RemoveAll(string.IsNullOrEmpty));
            VerifyPaxDetails(paxConfDetails.Select(GetPassenger));

        }

        private static void VerifyPaxDetails(IEnumerable<Passenger> passengers)
        {
            if (_passengers.Zip(passengers, (x, y) => x.Equals(y)).Any(x => x.Equals(false)))
                throw new ValidationException("Passenger Details");
        }

        private static Passenger GetPassenger(List<string> passengerElements)
        {
            var today = DateTime.Today;
            var bday = DateTime.Parse(passengerElements[passengerElements.IndexOf("BIRTHDATE") + 1]);
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
        
        private TripProduct ParseHotelTripProduct(IUIWebElement tripProduct)
        {
            var stayperiod = tripProduct.GetUIElements("stayDates").Select(x => x.Text).ToArray();
            return new HotelTripProduct()
            {
                ProductTitle = tripProduct.WaitAndGetBySelector("title", ApplicationSettings.TimeOut.Fast).Text,
                Address = tripProduct.WaitAndGetBySelector("subTitle", ApplicationSettings.TimeOut.Fast).Text,
                Rating = tripProduct.GetUIElements("rating").Count,
                StayPeriod = new StayPeriod()
                {
                    CheckInDate = DateTime.Parse(stayperiod[0]),
                    CheckOutDate = DateTime.Parse(stayperiod[1])
                },
                Fares = new Fare()
                {
                    TotalFare = new Amount(tripProduct.WaitAndGetBySelector("price", ApplicationSettings.TimeOut.Fast).Text)
                },
                Room = new HotelRoom()
                {
                    NoOfRooms = int.Parse(tripProduct.WaitAndGetBySelector("roomCount", ApplicationSettings.TimeOut.Fast).Text.Split()[0])
                }
            };
        }
        
        private List<TripProduct> ParseTripProducts()
        {
            return GetUIElements("tripProducts").Select(x =>
            {
                switch (GetTripProductType(x))
                {
                    case TripProductType.Air:
                        return ParseAirTripProduct(x);
                    case TripProductType.Hotel:
                        return ParseHotelTripProduct(x);
                    case TripProductType.Car:
                        return ParseCarTripProduct(x);
                    case TripProductType.Activity:
                        return ParseActivityTripProduct(x);
                    default:
                        return null;
                }
            }).ToList();
        }

        private TripProduct ParseActivityTripProduct(IUIWebElement paxCartContainer)
        {
            return new ActivityTripProduct()
                {
                    ProductTitle = paxCartContainer.WaitAndGetBySelector("activityTitle", ApplicationSettings.TimeOut.Fast).Text,
                    ActivityProductName = paxCartContainer.WaitAndGetBySelector("activityProductName", ApplicationSettings.TimeOut.Fast).Text,
                    Category = paxCartContainer.WaitAndGetBySelector("activityCategory", ApplicationSettings.TimeOut.Fast).Text,
                    Date = DateTime.Parse(paxCartContainer.WaitAndGetBySelector("stayDates", ApplicationSettings.TimeOut.Fast).Text),
                    Fares = new Fare()
                        {
                            TotalFare = new Amount(paxCartContainer.WaitAndGetBySelector("price", ApplicationSettings.TimeOut.Fast).Text)
                        }
                };
        }

        private TripProduct ParseAirTripProduct(IUIWebElement tripProduct)
        {
            LogManager.GetInstance().LogDebug("Parsing Air trip product on PassengerInfo Page");
            return new AirTripProduct()
            {
                Fares = new Fare() { TotalFare = new Amount(tripProduct.WaitAndGetBySelector("price", ApplicationSettings.TimeOut.Slow).Text.Trim()) },
                Airlines = tripProduct.GetUIElements("title").Select(x => x.Text.Trim()).ToList(),
                FlightLegs = ParseFlightLegs(tripProduct)
            };
        }

        private List<FlightLegs> ParseFlightLegs(IUIWebElement tripProduct)
        {
            var airportnames = tripProduct.GetUIElements("airportCodes").Select(x => x.Text).ToList();
            var legArrAirport = airportnames.Where((item, index) => index % 2 == 0).ToArray();
            var legDepAirport = airportnames.Where((item, index) => index % 2 != 0).ToArray();


            var legArrDepTime = tripProduct.GetUIElements("carTimes").Select(x =>
            {
                var a = x.Text.Split(' ');
                return a[1] + " " + a[2];
            }).ToList();
            var legArrTime = legArrDepTime.Where((item, index) => index % 2 == 0).ToArray();
            var legDepTime = legArrDepTime.Where((item, index) => index % 2 != 0).ToArray();

            var legArrDepDate = tripProduct.GetUIElements("carDates").Select(x => x.Text).ToList();
            var legArrDate = legArrDepDate.Where((item, index) => index != 0 && index % 4 == 1).ToArray();
            var legDepDate = legArrDepDate.Where((item, index) => index != 0 && index % 4 == 3).ToArray();

            return legArrAirport.Select((t, i) => new FlightLegs()
            {
                AirportPair = t + "-" + legDepAirport[i],
                ArriveTime = DateTime.Parse(legArrDate[i] + " " + legArrTime[i]),
                DepartTime = DateTime.Parse(legDepDate[i] + " " + legDepTime[i])
            }).ToList();
        }

        private TripProduct ParseCarTripProduct(IUIWebElement tripProduct)
        {
            var title = tripProduct.WaitAndGetBySelector("title", ApplicationSettings.TimeOut.Fast).Text.Split(' ');
            var totalFare = tripProduct.WaitAndGetBySelector("price", ApplicationSettings.TimeOut.Fast).Text;
            var carDates = tripProduct.GetUIElements("carDates").Select(x => x.Text).ToArray();
            var carTimes = tripProduct.GetUIElements("carTimes").Select(x => x.Text).ToArray();
            var pickUpDateTime = DateTime.Parse(carDates[1]).ToShortDateString() + " " + carTimes[0];
            var dropOffDateTime = DateTime.Parse(carDates[3]).ToShortDateString() + " " + carTimes[1];
            return new CarTripProduct()
            {
                RentalAgency = title[2],
                CarType = title[0],
                Fares = new Fare() { TotalFare = new Amount(totalFare) },
                PickUpDateTime = DateTime.Parse(pickUpDateTime),
                DropOffDateTime = DateTime.Parse(dropOffDateTime)
            };
        }

        private TripProductType GetTripProductType(IUIWebElement tripProduct)
        {
            if (tripProduct.WaitAndGetBySelector("hotelStars", ApplicationSettings.TimeOut.Fast) != null)
                return TripProductType.Hotel;
            var carTitle = tripProduct.WaitAndGetBySelector("title", ApplicationSettings.TimeOut.Fast);
            if (carTitle != null && carTitle.Text.Contains("Car"))
                return TripProductType.Car;
            if (tripProduct.WaitAndGetBySelector("activitiesIcon", ApplicationSettings.TimeOut.Fast) != null)
                return TripProductType.Activity;
            return TripProductType.Air;
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

        internal void ValidateTripDetails(Results selectedItinerary)
        {
            ParseTripProducts().ForEach(x=>this.ValidateTripProduct(x,selectedItinerary));
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

    }
}
