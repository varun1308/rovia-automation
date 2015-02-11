namespace Rovia.UI.Automation.Tests.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AppacitiveAutomationFramework;
    using OpenQA.Selenium;
    using Exceptions;
    using Logger;
    using ScenarioObjects;
    using ScenarioObjects.Activity;
    using ScenarioObjects.Hotel;
    using Configuration;
    using Validators;

    /// <summary>
    /// This class holds all the fields and methods for checkout page
    /// </summary>
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

        #region Public Properties

        public TripProductHolder TripProductHolder { get; set; }

        #endregion

        #region Public Members

        /// <summary>
        /// Payment with Credit card
        /// </summary>
        /// <param name="paymentMode"></param>
        public void PayNow(PaymentMode paymentMode)
        {
            SetPaymentMode(paymentMode);
            WaitAndGetBySelector("checkTerms", ApplicationSettings.TimeOut.Fast).Click();
            GetUIElements("paynow")[1].Click();
            WaitForPayment();
            CheckAndThrowErrors();
        }

        /// <summary>
        /// Wait for checkout page to load
        /// </summary>
        public void WaitForLoad()
        {
            IUIWebElement fillCcDetailsDiv;
            do
            {
                fillCcDetailsDiv = WaitAndGetBySelector("fillCCDetailsDiv", ApplicationSettings.TimeOut.Safe);
            } while (fillCcDetailsDiv == null || !fillCcDetailsDiv.Displayed);
        }

        /// <summary>
        /// Payment with Rovia Bucks
        /// </summary>
        /// <param name="paymentInfo"></param>
        public void PayNow(PaymentInfo paymentInfo)
        {
            SetPaymentMode(paymentInfo.PaymentMode);
            SetAddress(paymentInfo.BillingAddress);

            WaitAndGetBySelector("checkTerms", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("paynow", ApplicationSettings.TimeOut.Fast).Click();
        }

        /// <summary>
        /// Car Product book call
        /// </summary>
        public void BookNow()
        {
            WaitAndGetBySelector("checkTerms", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("booknow", ApplicationSettings.TimeOut.Fast).Click();
        }

        /// <summary>
        /// Check for payment status for success or failure
        /// </summary>
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

        /// <summary>
        /// Validate trip details on checkout page
        /// </summary>
        /// <param name="selectedItineary">added itinerary to cart</param>
        public void ValidateTripDetails(Results selectedItineary)
        {
            TripProductHolder.GetTripProducts().ForEach(x => this.ValidateTripProduct(x, selectedItineary));
        }

        /// <summary>
        /// validate booked product info on confirmation page to added product in cart
        /// </summary>
        /// <param name="selectedItineary">added itinerary to cart</param>
        public void ValidateBookedProductDetails(Results selectedItineary)
        {
            ParseBookedTripProducts().ForEach(x => this.ValidateBookedTripProducts(x, selectedItineary));
        }

        #endregion
        
    }
}
