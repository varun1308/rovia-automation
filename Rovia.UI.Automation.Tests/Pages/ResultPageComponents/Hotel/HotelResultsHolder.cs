using System.Collections.Generic;
using System.Linq;
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

        private  void SelectHotel(string hotelSupplier, string roomSupplier)
        {
            try
            {
                var suppliers = GetUIElements("suppliers").Where((x, i) => i % 3 == 2)
                            .Select(x => x.GetAttribute("title").Split('|')).ToArray();
                var validIndices = Enumerable.Range(0, suppliers.Count());
                if (!string.IsNullOrEmpty(hotelSupplier))
                    validIndices = validIndices.Where(x => suppliers[x][1].Equals(hotelSupplier));
                var resultIndex = validIndices.First(i => SelectRoom(GetUIElements("btnSelectHotel")[i], roomSupplier));
                _selectedItinerary.Supplier = new Supplier()
                    {
                        SupplierName = suppliers[resultIndex][1],
                        SupplierId = int.Parse(suppliers[resultIndex][0])
                    };
            }
            catch (StaleElementReferenceException)
            {
                LogManager.GetInstance().LogWarning("StaleElementReferenceException suppressed.");
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
            var ratings = GetUIElements("hotelRating").Select(x => x.GetUIElements("stars").Count).ToArray();
            var hotelNames = GetUIElements("divHotelName").Select(x=>x.Text);
            var prices = GetUIElements("hotelPrice").Select(x => new Amount(x.Text)).ToArray();
            return hotelNames.Select((x, index) => new HotelResult()
                {
                    HotelName = x,
                    Amount = prices[index],
                    HotelRating = ratings[index]
                } as Results).ToList();
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
                LogManager.GetInstance().LogWarning("No suitable results found on this page");
                return null;
            }
            catch (StaleElementReferenceException)
            {
                throw;
            }
        }
    }
}
