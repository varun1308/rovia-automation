using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.ScenarioObjects.Activity;
using Rovia.UI.Automation.ScenarioObjects.Air;
using Rovia.UI.Automation.ScenarioObjects.Hotel;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Pages.Parsers
{
    public class TripFolderParser : UIPage
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
                    BaseFare = i < baseFare.Length ? new Amount(baseFare[i]) : null,
                    Taxes = i < taxes.Length ? new Amount(taxes[i]) : null
                });
                i++;
            }
            return fares;
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
                case "ACTIVITY":
                    return ParseActivityTripProduct();
                default:
                    throw new InvalidInputException("ProductType : " + productType);
            }
        }

        private TripProduct ParseActivityTripProduct()
        {
            var activityDetails = GetUIElements("activityDetails");
            var paxInfoAndDate = GetUIElements("activityPaxDetails");
            return new ActivityTripProduct()
            {
                ActivityProductName = activityDetails[1].Text,
                Category = activityDetails[2].Text,
                Date = DateTime.Parse(paxInfoAndDate[0].Text),
                Passengers = new Passengers(paxInfoAndDate[1].Text)
            };
        }

        private TripProduct ParseAirTripProduct()
        {
            LogManager.GetInstance().LogDebug("Parsing Air trip product on TripFolder Page");
            return new AirTripProduct()
            {
                Passengers = new Passengers(WaitAndGetBySelector("totalPassengers", ApplicationSettings.TimeOut.Fast).Text.Replace("For", "")),
                Airlines = GetUIElements("productName").Select(x => x.Text.Trim()).ToList(),
                FlightLegs = GetFlightLegs()
            };
        }

        private List<FlightLeg> GetFlightLegs()
        {
            foreach (var t in GetUIElements("flightlegviewDetails").Where(t => t.Text.Contains("view")))
            {
                t.Click();
                Thread.Sleep(1000);
            }

            var segments = GetUIElements("legSegments");
            return GetUIElements("flightLegs").Select((x, i) => ParseFlightLeg(x, segments[i])).ToList();
        }

        private FlightLeg ParseFlightLeg(IUIWebElement leg, IUIWebElement segments)
        {
              var airportPair = leg.GetUIElements("legAirPorts").Select(x => x.Text).ToArray();
                var legInfo = leg.GetUIElements("legInfo").Select(x => x.Text.Replace("AM", "AM,").Replace("PM", "PM,").Split('\n').ToList()).ToList();
                legInfo.ForEach(x => x.RemoveAt(0));
                var segmentAirports = segments.GetUIElements("segmentAirPorts").Select(x => x.Text).ToArray();
                var segmentFlightdates = UtilityFunctions.GetDates(segments.GetUIElements("segmentTimes").Select(x => x.Text).ToList(), DateTime.Parse(string.Join(" ", legInfo[0])));
                var segmentDuration = segments.GetUIElements("segmentDuration").Select(x => x.Text.ToTimeSpan()).ToArray();
                var layOverDurations = segments.GetUIElements("layOverDuration").Select(x => x.Text.ToTimeSpan()).ToList();
                return new FlightLeg
                {
                    AirportPair = new AirportPair()
                    {
                        DepartureAirport = airportPair[0],
                        ArrivalAirport = airportPair[1],
                        DepartureDateTime = DateTime.Parse(string.Join(" ", legInfo[0])),
                        ArrivalDateTime = DateTime.Parse(string.Join(" ", legInfo[1])),
                    },
                    Cabin = legInfo[2][0].ToCabinType(),
                    LayOvers = layOverDurations,
                    Stops = layOverDurations.Count+segments.GetUIElements("noChangeStops").Count,
                    Segments = segments.GetUIElements("segmentAirLines").Select((x, i) => new FlightSegment()
                    {
                        AirLine = x.Text.Trim(),
                        Duration = segmentDuration[i],
                        AirportPair = new AirportPair()
                        {
                            DepartureAirport = segmentAirports[2 * i].Trim(),
                            ArrivalAirport = segmentAirports[2 * i + 1].Trim(),
                            DepartureDateTime = segmentFlightdates[2 * i],
                            ArrivalDateTime = segmentFlightdates[2 * i + 1]
                        }
                    }).ToList()
                };
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
            var carDates = GetUIElements("carDates").Select(x => x.Text).ToArray();
            var carTimes = GetUIElements("carTimes").Select(x => x.Text).ToArray();
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

        public List<TripProduct> ParseTripProducts()
        {
            var tripProducts = new List<TripProduct>();
            var productTypes = GetUIElements("pruductType").Select(x => x.GetAttribute("data-category")).ToArray();
            var productTitle = GetUIElements("productName").Select(x => x.Text).ToArray();
            var fares = ParseFares().ToArray();
            var modifyProductButton = GetUIElements("modifyItemClick").ToArray();
            var removeProductButton = GetUIElements("removeItemClick").ToArray();
            var passengers = GetUIElements("totalPassengers").Select(x => new Passengers(x.Text.Replace("For", ""))).ToArray();
            var i = 0;
            LogManager.GetInstance().LogDebug("Products on Trip Folder : " + string.Join("-", productTypes));
            while (i < productTypes.Length)
            {
                var product = ParseTripProduct(productTypes[i]);
                product.ProductTitle = productTitle[i];
                product.Fares = fares[i];
                product.Passengers = product.Passengers ?? (passengers.Length > 0 ? passengers[i] : null);
                product.ModifyProductButton = modifyProductButton[i];
                product.RemoveProductButton = removeProductButton[i];

                tripProducts.Add(product);
                i++;
            }
            return tripProducts;
        }


    }
}
