﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects.Hotel;
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

       private void ValidateTripProductDetails(HotelTripProduct hotelTripProduct, HotelResult hotelResult)
       {
           var errors = new StringBuilder();
           if (!hotelResult.HotelName.Equals(hotelTripProduct.ProductTitle))
               errors.Append(FormatError("HotelName", hotelResult.HotelName, hotelTripProduct.ProductTitle));
           if (!hotelResult.HotelAddress.Replace(",", "").Equals(hotelTripProduct.Address.Replace(",", "")))
               errors.Append(FormatError("HotelAddress", hotelResult.HotelAddress, hotelTripProduct.Address));
           if (!hotelResult.Amount.Equals(hotelTripProduct.Fares.TotalFare))
               errors.Append(FormatError("HotelPrice", hotelResult.Amount.ToString(), hotelTripProduct.Fares.TotalFare.ToString()));
           if (!hotelResult.StayPeriod.Equals(hotelTripProduct.StayPeriod))
               errors.Append(FormatError("StayPeriod", hotelResult.StayPeriod.ToString(), hotelTripProduct.StayPeriod.ToString()));
           if (!hotelResult.SelectedRoom.NoOfRooms.Equals(hotelTripProduct.Room.NoOfRooms))
               errors.Append(FormatError("NoOfRooms", hotelResult.SelectedRoom.NoOfRooms.ToString(), hotelTripProduct.Room.NoOfRooms.ToString()));
           if (!string.IsNullOrEmpty(errors.ToString()))
               throw new ValidationException(errors + "| on PaxInfoPage");
       }

       private string FormatError(string error, string addedValue, string tfValue)
       {
           return string.Format("| Invalid {0} ({1}, {2})", error, addedValue, tfValue);
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
                   TotalFare = new Amount(WaitAndGetBySelector("price", ApplicationSettings.TimeOut.Fast).Text)
               },
               Room = new HotelRoom()
               {
                   NoOfRooms = int.Parse(WaitAndGetBySelector("roomCount", ApplicationSettings.TimeOut.Fast).Text.Split()[0])
               }
           };
       }

       private void ValidateTripProductDetails(CarTripProduct carTripProduct, CarResult carResult)
        {
            var errors = new StringBuilder();
            if (!carResult.TotalPrice.Equals(carTripProduct.Fares.TotalFare))
                errors.Append(FormatError("CarFare", carResult.TotalPrice.ToString(), carTripProduct.Fares.TotalFare.ToString()));
            if (!carResult.CarType.Equals(carTripProduct.CarType))
                errors.Append(FormatError("CarType", carResult.CarType, carTripProduct.CarType));
            if (!carResult.RentalAgency.Equals(carTripProduct.RentalAgency))
                errors.Append(FormatError("RentalAgency", carResult.RentalAgency, carTripProduct.RentalAgency));
            if (!carResult.PickUpDateTime.Equals(carTripProduct.PickUpDateTime))
                errors.Append(FormatError("Pick Up DateTime", carResult.PickUpDateTime.ToLongDateString(), carTripProduct.PickUpDateTime.ToLongDateString()));
            if (!carResult.DropOffDateTime.Equals(carTripProduct.DropOffDateTime))
                errors.Append(FormatError("Drop Off DateTime", carResult.DropOffDateTime.ToLongDateString(), carTripProduct.DropOffDateTime.ToLongDateString()));
            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on PassengerInfoPage");
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
                    default:
                        return null;
                }
            }).ToList();
        }

        private TripProduct ParseAirTripProduct(IUIWebElement uiWebElement)
        {
            return new AirTripProduct();
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
            if (tripProduct.WaitAndGetBySelector("title", ApplicationSettings.TimeOut.Fast).Text.Contains("Car"))
                return TripProductType.Car;
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

       public void WaitForPageLoad()
       {
           while (WaitAndGetBySelector("SpinningDiv", 60).Displayed) ;
           if (WaitAndGetBySelector("divPassengerHolder", ApplicationSettings.TimeOut.Safe).Displayed == false)
               throw new PageLoadFailed("PassengerInfoPage");
       }

        internal void ValidateTripDetails(Results selectedItinerary)
       {
           ParseTripProducts().ForEach(x =>
               {
                   switch (x.ProductType)
                   {
                        case TripProductType.Air:
                           ValidateTripProductDetails(x as AirTripProduct, selectedItinerary as AirResult);
                            break;
                       case TripProductType.Hotel:
                            ValidateTripProductDetails(x as HotelTripProduct, selectedItinerary as HotelResult);
                           break;
                       case TripProductType.Car:
                           ValidateTripProductDetails(x as CarTripProduct, selectedItinerary as CarResult);
                           break;
                   }
               });
       }

        private void ValidateTripProductDetails(AirTripProduct airTripProduct, AirResult airResult)
        {
            
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
