using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Framework.Configurations;
using Rovia.UI.Automation.Framework.Utility;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.ScenarioObjects.Air;

namespace Rovia.UI.Automation.Framework.Pages
{
    /// <summary>
    /// Air Results page itineraries container
    /// </summary>
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
        
        private static Supplier ParseSupplier(IList<string> supplier)
        {
            return new Supplier()
            {
                SupplierId = int.Parse(supplier[0]),
                SupplierName = supplier[1]
            };
        }
        
        #endregion

        /// <summary>
        /// Check if results visible
        /// </summary>
        /// <returns></returns>
        #region Public Members

        public bool IsVisible()
        {
            var div = WaitAndGetBySelector("divMatrix", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }

        /// <summary>
        /// Add air product to cart
        /// </summary>
        /// <param name="criteria">criteria object to add itinerary</param>
        /// <returns></returns>
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
