using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppacitiveAutomationFramework;
using OpenQA.Selenium;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.ScenarioObjects.Activity;
using Rovia.UI.Automation.ScenarioObjects.Hotel;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Validators;

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
            LogManager.GetInstance().LogDebug("Parsing Air trip product on Checkout Page");
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
            var carTitle = tripProduct.WaitAndGetBySelector("title", ApplicationSettings.TimeOut.Fast);
            if (carTitle != null && carTitle.Text.Contains("Car"))
                return TripProductType.Car;
            if (tripProduct.WaitAndGetBySelector("activitiesIcon", ApplicationSettings.TimeOut.Fast) != null)
                return TripProductType.Activity;
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
                    case "Activity":
                        return ParseBookedActivityTripProduct(x);
                    default:
                        return null;
                }
            }).ToList();
        }

        private TripProduct ParseBookedActivityTripProduct(IUIWebElement tripProduct)
        {
            var activityDetails = tripProduct.GetUIElements("activityDetails").Select(x => x.Text).ToArray();
            var bookingDetails = tripProduct.GetUIElements("bProductDetails").Select(x => x.Text).ToArray();
            return new ActivityTripProduct()
            {
                ProductTitle = activityDetails[0],
                ActivityProductName = activityDetails[1],
                Category = activityDetails[2],
                Date = DateTime.Parse(bookingDetails[0]),
                Passengers = new Passengers(bookingDetails[1]),
                Fares = new Fare() { TotalFare = new Amount(tripProduct.WaitAndGetBySelector("price", ApplicationSettings.TimeOut.Fast).Text) }
            };
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
            var hotelDetails = tripProduct.GetUIElements("bookingDetails").Select(x => x.Text).ToArray();
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
            ParseTripProducts().ForEach(x=>this.ValidateTripProduct(x,selectedItineary));
        }

        public void ValidateBookedProductDetails(Results selectedItineary)
        {
            ParseBookedTripProducts().ForEach(x =>this.ValidateBookedTripProducts(x,selectedItineary));
        }

        

    }
}
