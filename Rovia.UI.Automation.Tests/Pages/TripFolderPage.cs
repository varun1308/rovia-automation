using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.ScenarioObjects.Hotel;
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
                    return ParseAirTripProduct();
                case "HOTEL":
                    return ParseHotelTripProduct();
                case "CAR":
                    return ParseCarTripProduct();
                default:
                    throw new InvalidInputException("ProductType : " + productType);
            }
        }

        private TripProduct ParseAirTripProduct()
        {
            LogManager.GetInstance().LogDebug("Parsing Air trip product on TripFolder Page");
            return new AirTripProduct()
                {
                    Fares = new Fare() { TotalFare = new Amount(WaitAndGetBySelector("totalFare", ApplicationSettings.TimeOut.Slow).Text.Trim()) },
                    Airlines = GetUIElements("productName").Select(x => x.Text.Trim()).ToList(),
                    FlightLegs = ParseFlightLegs()
                };
        }

        private List<FlightLegs> ParseFlightLegs()
        {
            foreach (var t in GetUIElements("flightlegviewDetails").Where(t => t.Text.Contains("view")))
            {
                t.Click();
                Thread.Sleep(1000);
            }

            var airportnames = GetUIElements("flightlegAirportCodes").Select(x => x.Text).ToList();
            var legArrAirport = airportnames.Where((item, index) => index % 2 == 0).ToArray();
            var legDepAirport = airportnames.Where((item, index) => index % 2 != 0).ToArray();
            var legArrDepTime = GetUIElements("legtimes").Select(x =>
            {
                var a = x.Text.Split(' ');
                return a[1] + " " + a[2];
            }).ToList();
            var legArrTime = legArrDepTime.Where((item, index) => index % 2 == 0).ToArray();
            var legDepTime = legArrDepTime.Where((item, index) => index % 2 != 0).ToArray();
            var segments = GetUIElements("flightAllSegments").ToList();

            var legCabins = GetUIElements("flightlegCabins").Where(x => x.Text.Contains("view fare rules")).
                    Select(x => x.Text.Split('|')[0].Split('(')[0]).ToArray();

            var legArrDepDate = GetUIElements("legDates").Select(x => x.Text).ToList();
            var legArrDate = legArrDepDate.Where((item, index) => index % 2 == 0).ToList();
            var legDepDate = legArrDepDate.Where((item, index) => index % 2 != 0).ToList();

            var arrDate = new List<string>();
            var depDate = new List<string>();

            for (var i = 0; i < segments.Count; i++)
            {
                var totalSegments = segments[i].GetUIElements("flightSegmentIdentifier").Where(x => x.Text.Contains("Take-off")).ToList();
                for (var j = 0; j < totalSegments.Count; j++)
                {
                    arrDate.Add(legArrDate[i]);
                    depDate.Add(legDepDate[i]);
                }
            }

            //GetUIElements("viewlegDetails").ForEach(x =>
            //    {
            //        x.Click();
            //        Thread.Sleep(1000);
            //    });
            //var legDuration = GetUIElements("legDuration").Select(x =>
            //{
            //    var arr = x.Text.Split();
            //       return (int.Parse(arr[0]) * 60 + int.Parse(arr[2] ?? "0"));
            //}).ToArray();

            //var legCabinAndStops = GetUIElements("legCabin").Select(x => x.Text).ToList();
            //var legCabins = legCabinAndStops.Select(x => x.Split(' ')[0]).Where((item, index) => index != 0 && (index == 2 || index % 6 == 0)).ToArray();

            return legArrAirport.Select((t, i) => new FlightLegs()
            {
                AirportPair = t + "-" + legDepAirport[i],
                ArriveTime = DateTime.Parse(arrDate[i] + " " + legArrTime[i]),
                DepartTime = DateTime.Parse(depDate[i] + " " + legDepTime[i])
            }).ToList();
        }

        private TripProduct ParseHotelTripProduct()
        {
            var elements = GetUIElements("hotelDetails").Select(x => x.Text).ToArray();
            return new HotelTripProduct()
                {
                    Address = WaitAndGetBySelector("hotelAddress", ApplicationSettings.TimeOut.Fast).Text,
                    StayPeriod = new StayPeriod()
                        {
                            CheckInDate = DateTime.Parse(elements[0]),
                            CheckOutDate = DateTime.Parse(elements[1])
                        },
                    Room = new HotelRoom()
                        {
                            Descriptions = elements[2]
                        },
                    KitchenType = elements[3]
                };
        }

        private TripProduct ParseCarTripProduct()
        {
            var carOptions = GetUIElements("carACnTransmission").ToArray();
            var carDates = GetUIElements("carDates").Select(x => x.Text).ToArray();//1 & 3
            var carTimes = GetUIElements("carTimes").Select(x => x.Text).ToArray();//0&1
            var pickUpDateTime = DateTime.Parse(carDates[0]).ToShortDateString() + " " + carTimes[0];
            var dropOffDateTime = DateTime.Parse(carDates[1]).ToShortDateString() + " " + carTimes[1];
            return new CarTripProduct()
                {
                    CarType = GetUIElements("productName").Select(x => x.Text.Split(' ')[0]).ToArray()[0],
                    AirConditioning = carOptions[1].Text,
                    Transmission = carOptions[2].Text,
                    PickUpDateTime = DateTime.Parse(pickUpDateTime),
                    DropOffDateTime = DateTime.Parse(dropOffDateTime)
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
            LogManager.GetInstance().LogDebug("Products on Trip Folder : " + string.Join("-", productTypes));
            while (i < productTypes.Length)
            {
                var product = ParseTripProduct(productTypes[i]);
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
            LogManager.GetInstance().LogDebug("Validating Air trip product on TripFolder Page");

            var errors = new StringBuilder();
            if (!airResult.Amount.Equals(airTripProduct.Fares.TotalFare))
                errors.Append(FormatError("Amount", airResult.Amount.ToString(), airTripProduct.Fares.TotalFare.ToString()));

            if (airTripProduct.FlightLegs.Count == airResult.Legs.Count)
            {
                for (var i = 0; i < airTripProduct.FlightLegs.Count; i++)
                {
                    if (!airTripProduct.FlightLegs[i].AirportPair.Equals(airResult.Legs[i].AirportPair))
                        errors.Append(FormatError("AirportPair", airResult.Legs[i].AirportPair, airTripProduct.FlightLegs[i].AirportPair));
                    if (!airTripProduct.FlightLegs[i].ArriveTime.Equals(airResult.Legs[i].ArriveTime))
                        errors.Append(FormatError("Arrival Time", airResult.Legs[i].ArriveTime.ToLongDateString(), airTripProduct.FlightLegs[i].ArriveTime.ToLongDateString()));
                    if (!airTripProduct.FlightLegs[i].DepartTime.Equals(airResult.Legs[i].DepartTime))
                        errors.Append(FormatError("Depart Time", airResult.Legs[i].DepartTime.ToLongDateString(), airTripProduct.FlightLegs[i].DepartTime.ToLongDateString()));
                    //need to parsing on result page first
                    //if (!airTripProduct.FlightLegs[i].Cabin.Equals(airResult.Legs[i].Cabin))
                    //    errors.Append(FormatError("Cabin", airResult.Legs[i].Cabin.ToString(), airTripProduct.FlightLegs[i].Cabin.ToString()));
                    //if (airTripProduct.FlightLegs[i].Duration != airResult.Legs[i].Duration && WaitAndGetBySelector("changeFlightTimes", ApplicationSettings.TimeOut.Fast) == null)
                    //    errors.Append(FormatError("Duration", airResult.Legs[i].Duration.ToString(), airTripProduct.FlightLegs[i].Duration.ToString()));
                }
            }
            else
                errors.Append(FormatError("Leg counts ", airResult.Legs.Count.ToString(), airTripProduct.FlightLegs.Count.ToString()));

            if (!airResult.AirLines.TrueForAll(airTripProduct.Airlines.Contains))
                errors.Append(FormatError("Airlines", string.Join(",", airResult.AirLines.ToArray()), string.Join(",", airTripProduct.Airlines.ToArray())));

            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on TripFolderPage");
        }

        private void ValidateTripProduct(HotelTripProduct hotelTripProduct, HotelResult hotelResult)
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
                throw new ValidationException(errors + "| on TripFolderPage");
        }

        private string FormatError(string error, string addedValue, string tfValue)
        {
            return string.Format("| Invalid {0} ({1}, {2})", error, addedValue, tfValue);
        }

        private void ValidateTripProduct(CarTripProduct carTripProduct, CarResult carResult)
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
            foreach (var tripProduct in ParseTripFolder().TripProducts)
            {
                LogManager.GetInstance().LogDebug("Validating trip product");
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
    }
}
