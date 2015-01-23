﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.ScenarioObjects.Air;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public class AirResultsHolder : UIPage, IResultsHolder
    {
        private AirResult _addedItinerary;

        #region Private Members

        private bool AddToCart(IUIWebElement flightSegmentHolder, IUIWebElement btnAddToCart)
        {
            var selectedItinerary = new AirResult()
                {
                    Legs = GetFlightLegs(flightSegmentHolder)
                };
            btnAddToCart.Click();
            try
            {
                while (true)
                {
                    GetUIElements("alerts").ForEach(x =>
                    {
                        if (x.Displayed)
                            throw new Alert(x.Text);
                    });

                    Thread.Sleep(1000);
                }
            }
            catch (Exception exception)
            {
                LogManager.GetInstance().LogWarning(exception.Message);
            }
            var btnCheckOut = WaitAndGetBySelector("btnCheckOut", ApplicationSettings.TimeOut.Slow);
            if (btnCheckOut != null && btnCheckOut.Displayed)
            {
                selectedItinerary.Amount = new Amount(WaitAndGetBySelector("priceOnCart", ApplicationSettings.TimeOut.Fast).Text);
                selectedItinerary.AirLines = new List<string>()
                    {
                        WaitAndGetBySelector("airlineOnCart", ApplicationSettings.TimeOut.Fast).Text
                    };
                btnCheckOut.Click();
                _addedItinerary = selectedItinerary;
                return true;
            }

            WaitAndGetBySelector("btnCancel", ApplicationSettings.TimeOut.Slow).Click();
            return false;

        }

        private List<FlightLeg> GetFlightLegs(IUIWebElement flightSegmentHolder)
        {
            foreach (var link in flightSegmentHolder.GetUIElements("viewDetailsLink"))
            {
                if (!link.Text.Equals("view details"))
                    continue;
                Thread.Sleep(1500);
                link.Click();
            }
            return flightSegmentHolder.GetUIElements("flightLegs").Select(ParseFlightLeg).ToList();

        }

        private FlightLeg ParseFlightLeg(IUIWebElement leg)
        {
            var airportPairs = leg.GetUIElements("legAirPorts").Select(x => x.Text).ToArray();
            var travelTimes = leg.GetUIElements("legTravelTime").Select(x => x.Text.Replace("AM", "AM,").Replace("PM", "PM,").Split('\n').ToList()).ToList();
            var segmentAirports = leg.GetUIElements("segmentAirPorts").Select(x => x.Text).ToArray();
            var segmentTimes = leg.GetUIElements("segmentTimes").Select(x => x.Text).ToArray();
            var segmentDuration = leg.GetUIElements("segmentDuration").Select(x => x.Text).ToArray();
            travelTimes.ForEach(x => x.RemoveAt(0));
            var legInfo = leg.GetUIElements("legInfo");
            var stopDetails = leg.GetUIElements("stops").Select(x => new
                {
                    IsFlightCahange = !x.Text.StartsWith("No"),
                    Date = DateTime.Parse(x.WaitAndGetBySelector("stopDate", ApplicationSettings.TimeOut.Fast).GetAttribute("textContent").Trim()),
                    Duration = x.WaitAndGetBySelector("stopDuration", ApplicationSettings.TimeOut.Fast).GetAttribute("textContent").ToTimeSpan()
                }).ToList();
            stopDetails.Insert(0, new
               {
                   IsFlightCahange = false,
                   Date = DateTime.Parse(string.Join(" ", travelTimes[0])),
                   Duration = TimeSpan.Zero
               });


            var segments = new List<FlightSegment>();
            var airLines = leg.GetUIElements("segmentAirLines");
            for (var j = 0; j < airLines.Count; j++)
            {
                var seg = new FlightSegment
                    {
                        AirLine = airLines[j].Text,
                        Duration = segmentDuration[j].ToTimeSpan(),
                        AirportPair = new AirportPair
                            {
                                DepartureAirport = segmentAirports[2 * j].Trim(),
                                ArrivalAirport = segmentAirports[2 * j + 1].Trim(),
                                DepartureDateTime =
                                    j > 0
                                        ? DateTime.Parse(segmentTimes[2 * j] + " " +
                                                         segments[j - 1].AirportPair.ArrivalDateTime.Add(
                                                             stopDetails[j].Duration)
                                                                        .ToShortDateString())
                                        : DateTime.Parse(segmentTimes[2 * j] + " " +
                                                         stopDetails[j].Date.Add(stopDetails[j].Duration)
                                                                       .ToShortDateString()),
                                ArrivalDateTime =
                                    DateTime.Parse(segmentTimes[2 * j + 1] + " " +
                                                   stopDetails[j + 1].Date.ToShortDateString())
                            }
                    };
                segments.Add(seg);
            }
            return new FlightLeg
                {
                    AirportPair = new AirportPair()
                        {
                            DepartureAirport = airportPairs[0],
                            ArrivalAirport = airportPairs[1],
                            DepartureDateTime = DateTime.Parse(string.Join(" ", travelTimes[0])),
                            ArrivalDateTime = DateTime.Parse(string.Join(" ", travelTimes[1])),
                        },
                    Duration =
                        leg.WaitAndGetBySelector("legDuration", ApplicationSettings.TimeOut.Fast).Text.Split(':')[1]
                            .ToTimeSpan(),
                    Cabin = legInfo[0].Text.ToCabinType(),
                    Stops = int.Parse(legInfo[1].Text),
                    LayOvers = stopDetails.Where(x => x.IsFlightCahange && x.Duration > TimeSpan.Zero).Select(y => y.Duration).ToList(),
                    Segments = segments
                };
        }

        private static void ProcessairLines(IList<string> airLines, IReadOnlyList<string> subair)
        {
            var j = 0;
            for (int i = 0; i < airLines.Count; i++)
            {
                if (airLines[i].Equals("Multiple Airlines"))
                    airLines[i] = subair[j++];
            }
        }

        private static AirResult ParseSingleResult(string perHeadPrice, string totalPrice, string airLine, List<FlightLeg> legs)
        {
            return new AirResult()
            {
                Amount = ParseAmount(perHeadPrice.Split(), totalPrice.Split()),
                AirLines = new List<string>(airLine.Split('/')),
                Legs = legs
            };
        }

        private static Supplier ParseSupplier(IList<string> supplier)
        {
            return new Supplier()
            {
                SupplierId = int.Parse(supplier[0]),
                SupplierName = supplier[1]
            };
        }

        private static Amount ParseAmount(IList<string> perHead, IList<string> total)
        {
            return new Amount()
            {
                Currency = perHead[1],
                TotalAmount = double.Parse(total[0].Remove(0, 1)),
                AmountPerPerson = double.Parse(perHead[0].Remove(0, 1))
            };
        }

        #endregion

        #region IResultHolder Members

        public bool IsVisible()
        {
            var div = WaitAndGetBySelector("divMatrix", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }

        public List<Results> ParseResults()
        {
            var price = GetUIElements("amount").Select(x => x.Text).ToArray();
            var airLines = GetUIElements("titleAirLines").Select(x => x.Text).ToList();
            var subair = GetUIElements("subTitleAirLines").Select(x => x.Text).ToList();
            var flightSegmentHolder = GetUIElements("flightSegmentHolder");
            ProcessairLines(airLines, subair);
            return airLines.Select((t, i) => ParseSingleResult(price[2 * i], price[2 * i + 1], t, GetFlightLegs(flightSegmentHolder[i]))).Cast<Results>().ToList();
        }

        public Results AddToCart(SearchCriteria criteria)
        {
            try
            {
                var suppliers = GetUIElements("suppliers").Select(x => ParseSupplier(x.GetAttribute("title").Split('|'))).ToArray();
                var btnAddToCart = GetUIElements("btnAddToCart");
                var flightSegmentHolder = GetUIElements("flightSegmentHolder");
                var validIndices = Enumerable.Range(0, suppliers.Count());
                if (!string.IsNullOrEmpty(criteria.Supplier))
                    validIndices = validIndices.Where(i => suppliers[i].SupplierName.Equals(criteria.Supplier));
                var resultIndex = validIndices.First(i => AddToCart(flightSegmentHolder[i], btnAddToCart[i]));
                _addedItinerary.Supplier = suppliers[resultIndex];
                _addedItinerary.Passengers = criteria.Passengers;
                return _addedItinerary;
            }
            catch (System.InvalidOperationException)
            {
                LogManager.GetInstance().LogWarning("No suitable results found on this page");
                return null;
            }
        }

        #endregion
    }
}
