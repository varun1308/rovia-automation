using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppacitiveAutomationFramework;
using OpenQA.Selenium;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    class HotelResultsHolder : UIPage, IResultsHolder
    {
        private Results _selectedItinerary;
        public HotelRoomsHolder RoomsHolder{private get; set;}
        public Action WaitingFunction { private get; set; }

        private  void SelectHotel(string hotelSupplier, string roomSupplier)
        {
            try
            {
                var suppliers = GetUIElements("suppliers").Where((x, i) => i % 3 == 2)
                            .Select(x => x.GetAttribute("title").Split('|')).ToArray();
                var validIndices = Enumerable.Range(0, suppliers.Count());
                if (!string.IsNullOrEmpty(hotelSupplier))
                    validIndices = validIndices.TakeWhile(x => suppliers[x][1].Equals(hotelSupplier));
                var resultIndex = validIndices.First(i => SelectRoom(GetUIElements("btnSelectHotel")[i], roomSupplier));
                _selectedItinerary.Supplier = new Supplier()
                    {
                        SupplierName = suppliers[resultIndex][1],
                        SupplierId = int.Parse(suppliers[resultIndex][0])
                    };
            }
            catch (StaleElementReferenceException exception)
            {
                throw;
            }
        }

        
        private bool SelectRoom(IUIWebElement btnSelectHotel, string supplier)
        {
            try
            {
                btnSelectHotel.Click();
                RoomsHolder.WaitForLoad();
                _selectedItinerary = RoomsHolder.AddToCart(supplier);
                return true;
            }
            catch (Alert alert)
            {
                LogManager.GetInstance().LogWarning(alert.Message);
                throw new AddToCartFailedException();
            }
        }

        public bool IsVisible()
        {
           var div = WaitAndGetBySelector("divResultHolder", ApplicationSettings.TimeOut.Slow);
           return div != null && div.Displayed;
        }

        public List<Results> ParseResults()
        {
            return null; //throw new NotImplementedException();
        }

        public Results AddToCart(string supplier)
        {
            try
            {
                var suppliers = supplier.Split('|');
                SelectHotel(suppliers[0], suppliers.Length == 2 ? suppliers[1] : null);
                return _selectedItinerary;
            }
            catch (System.InvalidOperationException)
            {
                LogManager.GetInstance().LogWarning("No result mactched addToCart Criteria");
                return null;
            }
            catch (StaleElementReferenceException exception)
            {
                throw;
            }
        }
    }
}
