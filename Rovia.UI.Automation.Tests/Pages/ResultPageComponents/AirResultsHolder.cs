using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public class AirResultsHolder : UIPage, IResultsHolder
    {
        private Dictionary<Results, IUIWebElement> _results;

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
            var supplier = GetUIElements("suppliers").Select(x => x.GetAttribute("title")).ToArray();
            var addToCartControl = GetUIElements("btnAddToCart");
            var flightLegs = ParseFlightLegs();
            var legsPerResult = flightLegs.Count / supplier.Length;
            ProcessairLines(airLines, subair);

            _results = new Dictionary<Results, IUIWebElement>();

            for (var i = 0; i < addToCartControl.Count; i++)
            {
                _results.Add(ParseSingleResult(price[2 * i], price[2 * i + 1], airLines[i], supplier[i], flightLegs.Skip(i * legsPerResult).Take(legsPerResult).ToList()), addToCartControl[i]);
            }
            return _results.Keys.ToList();
        }

        public void AddToCart(List<ScenarioObjects.Results> result)
        {
            if (result.Any(airResult => AddToCart(_results[airResult])))
            {
                return;
            }
            throw new Exception("Selected itineraries not Available");
        }
        #endregion

        #region Private Members
        private bool AddToCart(IUIWebElement btnAddToCart)
        {

            btnAddToCart.Click();
            var divloader = WaitAndGetBySelector("divLoader", ApplicationSettings.TimeOut.Fast);
            while (true)
            {
                if ((GetUIElements("alerts").Any(x => x.Displayed)))
                    break;
                Thread.Sleep(1000);
            }
            var btnCheckOut = WaitAndGetBySelector("btnCheckOut", ApplicationSettings.TimeOut.Slow);
            if (btnCheckOut != null && btnCheckOut.Displayed)
            {
                btnCheckOut.Click();
                return true;
            }

            WaitAndGetBySelector("btnCancel", ApplicationSettings.TimeOut.Slow).Click();
            return false;

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

        private static AirResult ParseSingleResult(string perHeadPrice, string totalPrice, string airLine, string supplier, List<FlightLegs> legs)
        {
            return new AirResult()
            {
                Amount = ParseAmount(perHeadPrice.Split(), totalPrice.Split()),
                AirLines = new List<string>(airLine.Split('/')),
                Supplier = ParseSupplier(supplier.Split('|')),
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

            var legCabinAndStops = GetUIElements("legCabinAndStop");
            var legCabins = legCabinAndStops.Select(x => x.Text).Where((item, index) => index % 2 == 0).ToArray();
            var legStops = legCabinAndStops.Select(x => x.Text).Where((item, index) => index % 2 != 0).ToArray();


            return legDuration.Select((t, i) => new FlightLegs()
                {
                    AirportPair = legArrAirport[i] + "-" + legDepAirport[i],
                    Duration = t,
                    ArriveTime = legArrTime[i],
                    DepartTime = legDepTime[i],
                    Cabin = GetCabinType(legCabins[i]),
                    Stops = int.Parse(legStops[i])
                }).ToList();
        }

        private CabinType GetCabinType(string cabinType)
        {
            if (cabinType.Contains(CabinType.Premium_Economy.ToString().Replace('_', ' ')))
                return CabinType.Premium_Economy;
            if (cabinType.Contains(CabinType.Business.ToString()))
                return CabinType.Business;
            if (cabinType.Contains(CabinType.First.ToString()))
                return CabinType.First;

            return CabinType.Economy;
        }

        #endregion
    }
}
