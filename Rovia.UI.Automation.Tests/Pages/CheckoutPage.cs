using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppacitiveAutomationFramework;
using OpenQA.Selenium;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.ScenarioObjects.Hotel;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages
{
    public class CheckoutPage : UIPage
    {
        #region Private Members

        private void SetPaymentMode(PaymentMode paymentMode)
        {
            var payOption = WaitAndGetBySelector(paymentMode == PaymentMode.RoviaBucks ? "roviaBucksCheck" : "creditCardCheck", ApplicationSettings.TimeOut.Fast);
            if (!payOption.Selected)
                payOption.Click();
        }

        private void WaitForPayment()
        {
            try
            {
                IUIWebElement preLoadingDiv;
                do
                {
                    preLoadingDiv = WaitAndGetBySelector("preloading", ApplicationSettings.TimeOut.Fast);
                } while (preLoadingDiv != null && preLoadingDiv.Displayed);
            }
            catch (StaleElementReferenceException)
            {
                LogManager.GetInstance().LogInformation("StaleElementReferenceException caught and suppressed");
                //eat OpenQASelenium.StaleElementReferenceException 
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
            catch (Exception)
            {
                LogManager.GetInstance().LogInformation("Set Address Failed");
                throw;
            }
        }

        private void CheckAndThrowErrors()
        {
            var divErrors = WaitAndGetBySelector("divErrors", ApplicationSettings.TimeOut.Slow);
            if (divErrors != null && divErrors.Displayed)
                throw new Alert(divErrors.Text);
        }

        private void ValidateTripProductDetails(HotelTripProduct hotelTripProduct, HotelResult hotelResult)
        {
            var errors = new StringBuilder();
            if (!hotelResult.HotelName.Equals(hotelTripProduct.ProductTitle,StringComparison.OrdinalIgnoreCase))
                errors.Append(FormatError("HotelName", hotelResult.HotelName, hotelTripProduct.ProductTitle));
            if (!hotelResult.HotelAddress.Replace(",", "").Equals(hotelTripProduct.Address.Replace(",", ""), StringComparison.OrdinalIgnoreCase))
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
                throw new ValidationException(errors + "| on CheckOutPage");
        }

        private void ValidateTripProductDetails(AirTripProduct airTripProduct, AirResult airResult)
        {
            //to Impletment
        }

        private string FormatError(string error, string addedValue, string tfValue)
        {
            return string.Format("| Invalid {0} ({1}, {2})", error, addedValue, tfValue);
        }

        private Fare ParseFares(IUIWebElement tripProduct)
        {
            return new Fare()
                {
                    TotalFare = new Amount(tripProduct.WaitAndGetBySelector("totalFare", ApplicationSettings.TimeOut.Fast).Text),
                    BaseFare = new Amount(tripProduct.WaitAndGetBySelector("baseFare", ApplicationSettings.TimeOut.Fast).Text.Split(':')[1].TrimStart(' ')),
                    Taxes = new Amount(tripProduct.GetUIElements("taxesandFees").ElementAt(1).Text.Split(':')[1].TrimStart(' '))
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
                    default:
                        return null;
                }
            }).ToList();
        }

        private TripProduct ParseAirTripProduct(IUIWebElement tripProduct)
        {
            //to implement
            return new AirTripProduct();
        }

        private TripProduct ParseCarTripProduct(IUIWebElement tripProduct)
        {
            var title = tripProduct.WaitAndGetBySelector("title", ApplicationSettings.TimeOut.Fast).Text.Split(' ');
            var totalFare = tripProduct.WaitAndGetBySelector("price", ApplicationSettings.TimeOut.Fast).Text;
            var carDates = tripProduct.GetUIElements("carDates").Select(x=>x.Text).ToArray();
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
                    NoOfRooms = int.Parse(tripProduct.WaitAndGetBySelector("roomCount", ApplicationSettings.TimeOut.Fast).Text.Split()[0])
                }
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

        private Passengers ParsePassengers(IUIWebElement tripProduct)
        {
            var passengers = tripProduct.WaitAndGetBySelector("bPassengers", ApplicationSettings.TimeOut.Fast).Text.Split(new[] { ' ', ',' }).ToList();
                return new Passengers()
                {
                    Adults = passengers.Contains("Adult") ? int.Parse(passengers[passengers.IndexOf("Adult") - 1]) : 0,
                    Children = passengers.Contains("Child") ? int.Parse(passengers[passengers.IndexOf("Child") - 1]) : 0,
                    Infants = passengers.Contains("Infant") ? int.Parse(passengers[passengers.IndexOf("Infant") - 1]) : 0
                };
            }


        #endregion

        #region Protected Members

        internal void PayNow(PaymentMode paymentMode)
        {
            SetPaymentMode(paymentMode);
            WaitAndGetBySelector("checkTerms", ApplicationSettings.TimeOut.Fast).Click();
            GetUIElements("paynow")[1].Click();
            WaitForPayment();
            CheckAndThrowErrors();
        }

        internal void WaitForLoad()
        {
            IUIWebElement fillCcDetailsDiv;
            do
            {
                fillCcDetailsDiv = WaitAndGetBySelector("fillCCDetailsDiv", ApplicationSettings.TimeOut.Safe);
            } while (fillCcDetailsDiv == null || !fillCcDetailsDiv.Displayed);
        }

        internal void PayNow(PaymentInfo paymentInfo)
        {
            SetPaymentMode(paymentInfo.PaymentMode);
            SetAddress(paymentInfo.BillingAddress);

            WaitAndGetBySelector("checkTerms", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("paynow", ApplicationSettings.TimeOut.Fast).Click(); 
        }

        internal void BookNow()
        {
            WaitAndGetBySelector("checkTerms", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("booknow", ApplicationSettings.TimeOut.Fast).Click(); 
        }

        #endregion

        public void CheckPaymentStatus()
        {
            try
            {
                do
                {
                    var paymentStatus = WaitAndGetBySelector("alertSuccess", ApplicationSettings.TimeOut.Safe);
                    if (paymentStatus != null && paymentStatus.Displayed)
                        break;
                    CheckAndThrowErrors();
                } while (true);
            }
            catch (Exception)
            {
                LogManager.GetInstance().LogInformation("Payment Failed"); 
                throw;
            }
        }

        public void ValidateTripDetails(Results selectedItineary)
        {
            ParseTripProducts().ForEach(x =>
            {
                switch (x.ProductType)
                {
                    case TripProductType.Air:
                        ValidateTripProductDetails(x as AirTripProduct, selectedItineary as AirResult);
                        break;
                    case TripProductType.Hotel:
                        ValidateTripProductDetails(x as HotelTripProduct, selectedItineary as HotelResult);
                        break;
                    case TripProductType.Car:
                       ValidateTripProductDetails(x as CarTripProduct, selectedItineary as CarResult);
                        break;
                }
            });
        }

        public void ValidateBookedProductDetails(Results selectedItineary)
        {
            ParseBookedTripProducts().ForEach(x =>
            {
                switch (x.ProductType)
                {
                    case TripProductType.Hotel:
                        ValidateBookedTripProducts(x as HotelTripProduct, selectedItineary as HotelResult);
                        break;
                    case TripProductType.Car:
                        ValidateBookedTripProducts(x as CarTripProduct, selectedItineary as CarResult);
                        break;
                }
            });
        }

        private void ValidateBookedTripProducts(HotelTripProduct hotelTripProduct, HotelResult hotelResult)
        {
            var errors = new StringBuilder();
            if (!hotelResult.HotelName.Equals(hotelTripProduct.ProductTitle, StringComparison.OrdinalIgnoreCase))
                errors.Append(FormatError("HotelName", hotelResult.HotelName, hotelTripProduct.ProductTitle));
            if (!hotelResult.HotelAddress.Replace(",", "").Equals(hotelTripProduct.Address.Replace(",", ""), StringComparison.OrdinalIgnoreCase))
                errors.Append(FormatError("HotelAddress", hotelResult.HotelAddress, hotelTripProduct.Address));
            if (!hotelResult.Amount.Equals(hotelTripProduct.Fares.TotalFare))
                errors.Append(FormatError("HotelPrice", hotelResult.Amount.ToString(), hotelTripProduct.Fares.TotalFare.ToString()));
            if (!hotelResult.StayPeriod.Equals(hotelTripProduct.StayPeriod))
                errors.Append(FormatError("StayPeriod", hotelResult.StayPeriod.ToString(), hotelTripProduct.StayPeriod.ToString()));
            if (!hotelResult.Passengers.Equals(hotelTripProduct.Passengers))
                errors.Append(FormatError("PassengersInfo", hotelResult.Passengers.ToString(), hotelTripProduct.Passengers.ToString()));
            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on ConfirmationPage");
        }
        private void ValidateBookedTripProducts(CarTripProduct carTripProduct, CarResult carResult)
        {
            var errors = new StringBuilder();
            if (!carResult.TotalPrice.Equals(carTripProduct.Fares.TotalFare))
                errors.Append(FormatError("CarFare", carResult.TotalPrice.ToString(), carTripProduct.Fares.TotalFare.ToString()));
            if (!carResult.CarType.Equals(carTripProduct.CarType))
                errors.Append(FormatError("CarType", carResult.CarType, carTripProduct.CarType));
            if (!carResult.AirConditioning.Equals(carTripProduct.AirConditioning))
                errors.Append(FormatError("AirConditioning", carResult.AirConditioning, carTripProduct.AirConditioning));
            if (!carResult.Transmission.Equals(carTripProduct.Transmission))
                errors.Append(FormatError("Transmission", carResult.Transmission, carTripProduct.Transmission));
            if (!carResult.PickUpDateTime.Equals(carTripProduct.PickUpDateTime))
                errors.Append(FormatError("Pick Up DateTime", carResult.PickUpDateTime.ToLongDateString(), carTripProduct.PickUpDateTime.ToLongDateString()));
            if (!carResult.DropOffDateTime.Equals(carTripProduct.DropOffDateTime))
                errors.Append(FormatError("Drop Off DateTime", carResult.DropOffDateTime.ToLongDateString(), carTripProduct.DropOffDateTime.ToLongDateString()));
            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on ConfirmationPage");
        }

        private List<TripProduct> ParseBookedTripProducts()
        {
            return GetUIElements("bookedProducts").Select(x =>
            {
                switch (x.GetAttribute("data-category").Trim())
                {
                    case "Hotel":
                        return ParseBookedHotelTripProduct(x);
                    case "Car":
                        return ParseBookedCarTripProduct(x);
                    default:
                        return null;
                }
            }).ToList();
        }

        private TripProduct ParseBookedCarTripProduct(IUIWebElement tripProduct)
        {
            var title = tripProduct.WaitAndGetBySelector("carTitleConfirmation", ApplicationSettings.TimeOut.Fast).Text.Split('-')[0].Split(' ')[0].TrimEnd(' ');
            var totalFare = tripProduct.WaitAndGetBySelector("totalFare", ApplicationSettings.TimeOut.Fast).Text;
            var pickUp = tripProduct.GetUIElements("carPickUpDateTimeConfirmation").Select(x => x.Text).ToArray();
            var pickUpDateTime = DateTime.Parse(pickUp[0]).ToShortDateString() + " " + pickUp[1];
            var dropoff = tripProduct.GetUIElements("carDropoffDateTimeConfirmation").Select(x => x.Text).ToArray();
            var dropOffDateTime = DateTime.Parse(dropoff[0]).ToShortDateString() + " " + dropoff[1];
            var carOptions = tripProduct.GetUIElements("carOptionsConfirmation").Select(x => x.Text).ToArray();
            var passengerName = tripProduct.WaitAndGetBySelector("carPassengersConfirmation", ApplicationSettings.TimeOut.Fast).Text;
            return new CarTripProduct()
            {
                CarType = title,
                AirConditioning = carOptions[1],
                Transmission = carOptions[2],
                Fares = new Fare() { TotalFare = new Amount(totalFare) },
                PickUpDateTime = DateTime.Parse(pickUpDateTime),
                DropOffDateTime = DateTime.Parse(dropOffDateTime)
            };
        }

        private TripProduct ParseBookedHotelTripProduct(IUIWebElement tripProduct)
        {
            var hotelDetails = tripProduct.GetUIElements("bHotelDetails").Select(x => x.Text).ToArray();
            return new HotelTripProduct()
            {
                ProductTitle = tripProduct.WaitAndGetBySelector("bProductTitle", ApplicationSettings.TimeOut.Fast).Text,
                Address = tripProduct.WaitAndGetBySelector("bSubTitle", ApplicationSettings.TimeOut.Fast).Text,
                StayPeriod = new StayPeriod()
                        {
                            CheckInDate = DateTime.Parse(hotelDetails[0]),
                            CheckOutDate = DateTime.Parse(hotelDetails[1])
                        },
                Room = new HotelRoom()
                    {
                        Descriptions = hotelDetails[2]
                    },
                Passengers = ParsePassengers(tripProduct),
                KitchenType = hotelDetails[3],
                Fares = ParseFares(tripProduct)
            };
        }

    }
}
