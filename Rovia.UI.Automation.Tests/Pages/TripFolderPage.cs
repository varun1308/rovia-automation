using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Utility;


namespace Rovia.UI.Automation.Tests.Pages
{
    public class TripFolderPage : UIPage
    {
        private IEnumerable<Fare> ParseFares()
        {
            var fares = new List<Fare>();
            var i = 0;
            var totalFare = GetUIElements("totalFare").Select(x => x.Text).ToArray();
            var baseFare =
                GetUIElements("fareBreakup")
                   .Where((item) => item.Text.Contains("Base "))
                    .Select(x => x.Text.Split(':')[1].TrimStart(' '))
                    .ToArray();
            var taxes =
                GetUIElements("fareBreakup")
                    .Where((item) => item.Text.Contains("Taxes & fees"))
                    .Select(x => x.Text.Split(':')[1].TrimStart(' '))
                    .ToArray();

            while (i < totalFare.Length)
            {
               fares.Add(new Fare()
               {
                   TotalFare = new Amount(totalFare[i]),
                   BaseFare = new Amount(baseFare[i]),
                   Taxes = new Amount(taxes[i])
               });
                i++;
            }
            return fares;
        }

        private IEnumerable<Passengers> ParsePassengers()
        {
            return GetUIElements("totalPassengers").Select(x =>
            {

                var passengers = x.Text.Split(new[] { ' ', ',' }).ToList();
                return new Passengers()
                {
                    Adults = passengers.Contains("Adult") ? int.Parse(passengers[passengers.IndexOf("Adult") - 1]) : 0,
                    Children = passengers.Contains("Child") ? int.Parse(passengers[passengers.IndexOf("Child") - 1]) : 0,
                    Infants = passengers.Contains("Infant") ? int.Parse(passengers[passengers.IndexOf("Infant") - 1]) : 0
                };
            });
        }

        private TripProduct ParseTripProduct(string productType)
        {
            switch (productType.ToUpper())
            {
                case "AIR":
                case "FLIGHT":
                    return null;
                case "HOTEL":
                    return ParseHotelTripProduct();
                case "CAR":
                    return ParseCarTripProduct();
                default:
                    throw new InvalidInputException("ProductType : "+productType);
            }
        }

        private TripProduct ParseHotelTripProduct()
        {
            var elements = GetUIElements("hotelDetails").Select(x=>x.Text).ToArray();
            return new HotelTripProduct()
                {
                    Address = WaitAndGetBySelector("hotelAddress",ApplicationSettings.TimeOut.Fast).Text,
                    StayPeriod = new StayPeriod()
                        {
                            CheckInDate = DateTime.Parse(elements[0]),
                            CheckOutDate = DateTime.Parse(elements[1])
                        },
                    Room = elements[2],
                    KitchenType = elements[3]
                };
        }

        private TripProduct ParseCarTripProduct()
        {
            var carOptions = GetUIElements("carACnTransmission").ToArray();
            return new CarTripProduct()
                {
                    CarType = GetUIElements("productName").Select(x => x.Text.Split(' ')[0]).ToArray()[0],
                    AirConditioning = carOptions[1].Text,
                    Transmission = carOptions[2].Text
                    //to do impletentation for Rental Agency
                };
        }

        private List<TripProduct> ParseTripProducts()
        {
            var tripProducts = new List<TripProduct>();
            var productTypes = GetUIElements("pruductType").Select(x => x.GetAttribute("data-category")).ToArray();
            var productTitle = GetUIElements("productName").Select(x => x.Text).ToArray();
            var fares = ParseFares().ToArray();
            var modifyProductButton = GetUIElements("modifyItemClick").ToArray();
            var removeProductButton = GetUIElements("removeItemClick").ToArray();
            var passengers = ParsePassengers().ToArray();
            var i = 0;
            while (i < productTypes.Length)
            {
                var product=ParseTripProduct(productTypes[i]);
                product.ProductTitle = productTitle[i];
                product.Fares = fares[i];
                product.Passengers = passengers.Length > 0 ? passengers[i] : null;
                product.ModifyProductButton = modifyProductButton[i];
                product.RemoveProductButton = removeProductButton[i];

                tripProducts.Add(product);
                i++;
            }
            return tripProducts;
        }

        private TripFolder ParseTripFolder()
        {
            var trip = new TripFolder()
            {
                TripProducts = ParseTripProducts(),
            };
            return trip;
        }

        private void ValidateTripProduct(AirTripProduct airTripProduct, AirResult airResult)
        {
            //throw new NotImplementedException();
        }

        private void ValidateTripProduct(HotelTripProduct hotelTripProduct, HotelResult hotelResult)
        {
            var errors = new StringBuilder();
            if (!hotelResult.HotelName.Equals(hotelTripProduct.ProductTitle))
                errors.Append(FormatError("HotelName",hotelResult.HotelName,hotelTripProduct.ProductTitle));
            if (!hotelResult.HotelAddress.Replace(",", "").Equals(hotelTripProduct.Address.Replace(",", "")))
                errors.Append(FormatError("HotelAddress", hotelResult.HotelAddress, hotelTripProduct.Address));
            if (!hotelResult.Amount.Equals(hotelTripProduct.Fares.TotalFare))
                errors.Append(FormatError("HotelPrice", hotelResult.Amount.ToString(), hotelTripProduct.Fares.TotalFare.ToString()));
            if (!hotelResult.StayPeriod.Equals(hotelTripProduct.StayPeriod))
                errors.Append(FormatError("StayPeriod", hotelResult.StayPeriod.ToString(), hotelTripProduct.StayPeriod.ToString()));
            if (!hotelResult.Passengers.Equals(hotelTripProduct.Passengers))
                errors.Append(FormatError("PassengersInfo", hotelResult.Passengers.ToString(), hotelTripProduct.Passengers.ToString()));
            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on TripFolderPage");
        }

        private string FormatError(string error, string addedValue, string tfValue)
        {
            return string.Format("| Invalid {0} ({1}, {2})",error,addedValue,tfValue);
        }

        private void ValidateTripProduct(CarTripProduct carTripProduct, CarResult carResult)
        {
            var errors = new StringBuilder();
            if (!carResult.TotalPrice.Equals(carTripProduct.Fares.TotalFare))
                errors.Append(FormatError("CarFare",carResult.TotalPrice.ToString(),carTripProduct.Fares.TotalFare.ToString()));
            if (!carResult.CarType.Equals(carTripProduct.CarType))
                errors.Append(FormatError("CarType", carResult.CarType, carTripProduct.CarType));
            if (!carResult.AirConditioning.Equals(carTripProduct.AirConditioning))
                errors.Append(FormatError("AirConditioning", carResult.AirConditioning,carTripProduct.AirConditioning));
            if (!carResult.Transmission.Equals(carTripProduct.Transmission))
                errors.Append(FormatError("Transmission",carResult.Transmission,carTripProduct.Transmission));
            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on TripFolderPage");
        }


        internal void EditTripName()
        {
            WaitAndGetBySelector("tripSettings", ApplicationSettings.TimeOut.Fast).Click();

            WaitAndGetBySelector("editTripName", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("tripNameTextbox", ApplicationSettings.TimeOut.Slow).SendKeys("EditedTripName");

            //if wants to test cancel button/closing popup funcionality uncomment below & comment savetrip click 
            //WaitAndGetBySelector("cancelEditTripName",ApplicationSettings.TimeOut.Fast).Click();

            WaitAndGetBySelector("SaveTripName", ApplicationSettings.TimeOut.Fast).Click();
        }

        internal void ValidateTripFolder(Results selectedItineary)
        {
            var tripProduct = ParseTripFolder().TripProducts[0];
            switch (tripProduct.ProductType)
            {
                case TripProductType.Air:
                    ValidateTripProduct(tripProduct as AirTripProduct, selectedItineary as AirResult);
                    break;
                case TripProductType.Hotel:
                    ValidateTripProduct(tripProduct as HotelTripProduct, selectedItineary as HotelResult);
                    break;
                case TripProductType.Car:
                    ValidateTripProduct(tripProduct as CarTripProduct, selectedItineary as CarResult);
                    break;
                case TripProductType.Activity:
                    break;
            }
        }

        internal void CheckoutTrip()
        {
            WaitAndGetBySelector("checkoutButton", ApplicationSettings.TimeOut.Slow).Click();
        }

        internal void ContinueShopping()
        {
            WaitAndGetBySelector("continueShopping", ApplicationSettings.TimeOut.Slow).Click();
        }
       
        internal void TripStartOver()
        {
            WaitAndGetBySelector("tripSettings", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("startOver", ApplicationSettings.TimeOut.Fast).Click();
        }
       
        internal void SaveTrip()
        {
            WaitAndGetBySelector("tripSettings", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("saveTripButton", ApplicationSettings.TimeOut.Fast).Click();
        }
    }
}
