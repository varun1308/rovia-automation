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

        private void ValidateHotelTripProductDetails(HotelTripProduct hotelTripProduct, HotelResult hotelResult)
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

        private List<TripProduct> ParseTripProducts()
        {
            return GetUIElements("tripProducts").Select(x =>
            {
                switch (GetTripProductType(x))
                {
                    case TripProductType.Hotel:
                        return ParseHotelTripProduct(x);
                    default:
                        return null;
                }
            }).ToList();
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

        private TripProductType GetTripProductType(IUIWebElement tripProduct)
        {
            if (tripProduct.WaitAndGetBySelector("hotelStars", ApplicationSettings.TimeOut.Fast) != null)
                return TripProductType.Hotel;
            return TripProductType.Air;
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
            }while (fillCcDetailsDiv == null || !fillCcDetailsDiv.Displayed) ;
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
                    case TripProductType.Hotel:
                        ValidateHotelTripProductDetails(x as HotelTripProduct, selectedItineary as HotelResult);
                        break;
                }
            });
        }
    }
}
