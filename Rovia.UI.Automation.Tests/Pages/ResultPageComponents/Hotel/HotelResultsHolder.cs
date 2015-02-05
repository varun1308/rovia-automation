namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    using System.Linq;
    using AppacitiveAutomationFramework;
    using OpenQA.Selenium;
    using Criteria;
    using Exceptions;
    using Logger;
    using ScenarioObjects;
    using Configuration;

    /// <summary>
    /// Hotel Results page itineraries container
    /// </summary>
    class HotelResultsHolder : UIPage, IResultsHolder
    {
        private Results _selectedItinerary;
        public HotelRoomsHolder RoomsHolder { private get; set; }

        #region Private Members

        private  void SelectHotel(string hotelSupplier, string roomSupplier)
        {
            try
            {
                var suppliers = GetUIElements("suppliers").Where((x, i) => i % 3 == 2)
                            .Select(x => x.GetAttribute("title").Split('|')).ToArray();
                var validIndices = Enumerable.Range(0, suppliers.Count());
                if (!string.IsNullOrEmpty(hotelSupplier))
                    validIndices = validIndices.Where(x => suppliers[x][1].Equals(hotelSupplier));
                var btnAddToCart = GetUIElements("btnSelectHotel");
                var resultIndex = validIndices.First(i => SelectRoom(btnAddToCart[i], roomSupplier));
                _selectedItinerary.Supplier = new Supplier()
                    {
                        SupplierName = suppliers[resultIndex][1],
                        SupplierId = int.Parse(suppliers[resultIndex][0])
                    };
            }
            catch (StaleElementReferenceException)
            {
                LogManager.GetInstance().LogWarning("StaleElementReferenceException suppressed.");
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

        #endregion

        #region Public Members

        /// <summary>
        /// Check if results visible
        /// </summary>
        /// <returns></returns>
        public bool IsVisible()
        {
           var div = WaitAndGetBySelector("divResultHolder", ApplicationSettings.TimeOut.Slow);
           return div != null && div.Displayed;
        }

        /// <summary>
        /// Select hotel
        /// </summary>
        /// <param name="criteria">criteria object to add itinerary</param>
        /// <returns></returns>
        public Results AddToCart(SearchCriteria  criteria)
        {
            try
            {
                var suppliers = criteria.Supplier.Split('|');
                SelectHotel(suppliers[0], suppliers.Length == 2 ? suppliers[1] : null);
                return _selectedItinerary;
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
