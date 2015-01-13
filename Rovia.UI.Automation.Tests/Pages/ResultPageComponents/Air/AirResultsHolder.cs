using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public class AirResultsHolder : UIPage, IResultsHolder
    {
        private AirResult _addedItinerary;

        #region Private Members

        private bool AddToCart(IUIWebElement btnAddToCart)
        {

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
                _addedItinerary = ParseResultFromCart();
                btnCheckOut.Click();
                return true;
            }

            WaitAndGetBySelector("btnCancel", ApplicationSettings.TimeOut.Slow).Click();
            return false;

        }

        private AirResult ParseResultFromCart()
        {
            return new AirResult()
                {
                    Amount = new Amount(WaitAndGetBySelector("priceOnCart", ApplicationSettings.TimeOut.Fast).Text),
                    AirLines = new List<string>() { WaitAndGetBySelector("airlineOnCart", ApplicationSettings.TimeOut.Fast).Text },
                    Legs = ParseFlightLegOnCart()
                };
        }

        private List<FlightLegs> ParseFlightLegOnCart()
        {
            var airportnames = GetUIElements("flightAirportsOnCart").Select(x => x.Text.Substring(0, 3)).ToList();
            var legArrAirport = airportnames.Where((item, index) => index % 2 == 0).ToArray();
            var legDepAirport = airportnames.Where((item, index) => index % 2 != 0).ToArray();

            var legArrDepTime = GetUIElements("flightTimesOnCart").Select(x => x.Text).ToList();
            var legArrTime = legArrDepTime.Where((item, index) => index % 2 == 0).ToArray();
            var legDepTime = legArrDepTime.Where((item, index) => index % 2 != 0).ToArray();

            var legArrDepDate = GetUIElements("flightDatesOnCart").Select(x => x.Text).ToList();
            var legArrDate = legArrDepDate.Where((item, index) => index % 2 == 0).ToArray();
            var legDepDate = legArrDepDate.Where((item, index) => index % 2 != 0).ToArray();

            return legArrAirport.Select((t, i) => new FlightLegs()
            {
                AirportPair = t + "-" + legDepAirport[i],
                ArriveTime = DateTime.Parse(legArrDate[i] + " " + legArrTime[i]),
                DepartTime = DateTime.Parse(legDepDate[i] + " " + legDepTime[i])
            }).ToList();
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

        private static AirResult ParseSingleResult(string perHeadPrice, string totalPrice, string airLine, List<FlightLegs> legs)
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

        private List<FlightLegs> ParseFlightLegs()
        {
            var airportnames = GetUIElements("legAirports");
            var legArrAirport = airportnames.Select(x => x.Text).Where((item, index) => index % 2 == 0).ToArray();
            var legDepAirport = airportnames.Select(x => x.Text).Where((item, index) => index % 2 != 0).ToArray();
            var legDuration = GetUIElements("legDuration").Select(x =>
            {
                var arr = x.Text.Split();
                return (int.Parse(arr[1]) * 60 + int.Parse(arr[3] ?? "0"));
            }).ToArray();
            var legArrDepTime = GetUIElements("legArrDepTime");
            var legArrTime = legArrDepTime.Select(x => x.Text).Where((item, index) => index % 2 == 0).ToArray();
            var legDepTime = legArrDepTime.Select(x => x.Text).Where((item, index) => index % 2 != 0).ToArray();

            var legArrDepDate = GetUIElements("legArrDepDate");
            var legArrDate = legArrDepDate.Select(x => x.Text).Where((item, index) => index % 2 == 0).ToArray();
            var legDepDate = legArrDepDate.Select(x => x.Text).Where((item, index) => index % 2 != 0).ToArray();

            var legCabinAndStops = GetUIElements("legCabinAndStop");
            var legCabins = legCabinAndStops.Select(x => x.Text).Where((item, index) => index % 2 == 0).ToArray();
            var legStops = legCabinAndStops.Select(x => x.Text).Where((item, index) => index % 2 != 0).ToArray();


            return legDuration.Select((t, i) => new FlightLegs()
                {
                    AirportPair = legArrAirport[i] + "-" + legDepAirport[i],
                    Duration = t,
                    ArriveTime = DateTime.Parse(legArrDate[i] + " " + legArrTime[i]),
                    DepartTime = DateTime.Parse(legDepDate[i] + " " + legDepTime[i]),
                    Cabin = legCabins[i].Split()[0].ToCabinType(),
                    Stops = int.Parse(legStops[i])
                }).ToList();
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
            var flightLegs = ParseFlightLegs();
            ProcessairLines(airLines, subair);
            var legsPerResult = flightLegs.Count / airLines.Count;
            return airLines.Select((t, i) => ParseSingleResult(price[2*i], price[2*i + 1], t, flightLegs.Skip(i*legsPerResult).Take(legsPerResult).ToList())).Cast<Results>().ToList();
        }

        public Results AddToCart(SearchCriteria criteria)
        {
            try
            {
                var suppliers = GetUIElements("suppliers").Select(x => ParseSupplier(x.GetAttribute("title").Split('|'))).ToArray();
                var btnAddToCart = GetUIElements("btnAddToCart");
                var validIndices = Enumerable.Range(0, suppliers.Count());
                if (!string.IsNullOrEmpty(criteria.Supplier))
                    validIndices = validIndices.Where(i => suppliers[i].SupplierName.Equals(criteria.Supplier));
                var resultIndex = validIndices.First(i => AddToCart(btnAddToCart[i]));
                _addedItinerary.Supplier = suppliers[resultIndex];
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
