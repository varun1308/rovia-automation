using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.ScenarioObjects.Activity;
using Rovia.UI.Automation.ScenarioObjects.Air;
using Rovia.UI.Automation.ScenarioObjects.Hotel;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages
{
    public class TripProductHolder : UIPage
    {
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
                FlightLegs = new List<FlightLeg>()
                    {
                        new FlightLeg()
                            {
                                Segments = ParseFlightSegments()
                            }
                    }
            };
        }

        private List<FlightSegment> ParseFlightSegments()
        {
            var airportCodes = GetUIElements("airportCodes");
            var flightTimes = GetUIElements("flightTimes").Select(x => x.Text.Replace("AM", "AM,").Replace("PM", "PM,").Split('\n').ToList()).ToList();
            flightTimes.ForEach(x => x.RemoveAt(0));
            var airSegments = GetUIElements("title").Select((x, i) => new FlightSegment()
                {
                    AirLine = x.Text,
                    AirportPair = new AirportPair()
                        {
                            DepartureAirport = airportCodes[2 * i].Text.Trim(),
                            ArrivalAirport = airportCodes[2 * i + 1].Text.Trim(),
                            DepartureDateTime = DateTime.Parse(string.Join(" ", flightTimes[2 * i]).Remove(0, 3)),
                            ArrivalDateTime = DateTime.Parse(string.Join(" ", flightTimes[2 * i + 1]).Remove(0, 3)),
                        }
                }).ToList();
            return airSegments;
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

        public List<TripProduct> GetTripProducts()
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

    }
}
