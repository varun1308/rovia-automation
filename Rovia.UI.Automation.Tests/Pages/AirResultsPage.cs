using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Pages
{
    public class AirResultsPage : UIPage,IResultsPage
    {
        
        private static Dictionary<Results, IUIWebElement> _results;

        #region private member functions
        private bool IsWaitingVisible()
        {
            var div = WaitAndGetBySelector("divWaiting", ApplicationSettings.TimeOut.Fast);
            return div != null && div.Displayed;
        }

        private bool IsResultsVisible()
        {
            var div = WaitAndGetBySelector("divMatrix", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }

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
            if (btnCheckOut != null && btnCheckOut.Displayed )
            {
                btnCheckOut.Click();
                return true;
            }

            WaitAndGetBySelector("btnCancel", ApplicationSettings.TimeOut.Slow).Click();
            return false;

        }

        private void ProcessairLines(List<string> airLines, List<string> subair)
        {
            var j = 0;
            for (int i = 0; i < airLines.Count; i++)
            {
                if (airLines[i].Equals("Multiple Airlines"))
                    airLines[i] = subair[j++];
            }
        }

        private AirResult ParseSingleResult(string perHeadPrice, string totalPrice, string airLine, string supplier, List<FlightLegs> legs)
        {
            return new AirResult()
            {
                Amount = ParseAmount(perHeadPrice.Split(), totalPrice.Split()),
                AirLines = new List<string>(airLine.Split('/')),
                Supplier = ParseSupplier(supplier.Split('|'))
                //todo implemet leg parsing
            };
        }

        private Supplier ParseSupplier(string[] supplier)
        {
            return new Supplier()
            {
                SupplierId = int.Parse(supplier[0]),
                SupplierName = supplier[1]
            };
        }

        private Amount ParseAmount(string[] perHead, string[] total)
        {
            return new Amount()
            {
                Currency = perHead[1],
                TotalAmount = double.Parse(total[0].Remove(0, 1)),
                AmountPerPerson = double.Parse(perHead[0].Remove(0, 1))
            };
        } 
        #endregion

        #region IResultsPage Member Functions
        public void AddToCart(List<Results> result)
        {

            if (result.Any(airResult => AddToCart(_results[airResult])))
            {
                return;
            }
            throw new Exception("AddToCartFailed");
        }

        public void WaitForResultLoad()
        {
            try
            {
                while (IsWaitingVisible())
                {
                    Thread.Sleep(2000);
                    if (IsResultsVisible())
                        break;
                }
                if (!IsResultsVisible())
                    throw new Exception("Results Not visible");
            }
            catch (Exception exception)
            {
                throw new Exception("Results failed to load", exception);
            }
        }

        public List<Results> ParseResults()
        {

            var price = GetUIElements("amount").Select(x => x.Text).ToArray();
            var airLines = GetUIElements("titleAirLines").Select(x => x.Text).ToList();
            var subair = GetUIElements("subTitleAirLines").Select(x => x.Text).ToList();
            var supplier = GetUIElements("suppliers").Select(x => x.GetAttribute("title")).ToArray();
            var addToCartControl = GetUIElements("btnAddToCart");
            ProcessairLines(airLines, subair);
            _results = new Dictionary<Results, IUIWebElement>();
            for (var i = 0; i < addToCartControl.Count; i++)
            {
                _results.Add(ParseSingleResult(price[2 * i], price[2 * i + 1], airLines[i], supplier[i], null), addToCartControl[i]);
            }
            return _results.Keys.ToList();
        } 
        #endregion

        
    }
}
