namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    using System;
    using System.Linq;
    using System.Threading;
    using AppacitiveAutomationFramework;
    using Criteria;
    using Exceptions;
    using Logger;
    using ScenarioObjects;
    using Configuration;

    /// <summary>
    /// Car Results page itineraries container
    /// </summary>
    public class CarResultHolder : UIPage, IResultsHolder
    {
        private CarResult _addedItinerary;

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

        private CarResult ParseResultFromCart()
        {
            var carOptions = GetUIElements("carOptionsOnCart");
            var dates = GetUIElements("DatesOnCart").Select(x => x.Text).ToArray();
            var rental = WaitAndGetBySelector("rentalAgencyOnCArt", ApplicationSettings.TimeOut.Fast).GetAttribute("alt");
            return new CarResult()
                {
                    RentalAgency = rental,
                    AirConditioning = carOptions[1].Text,
                    Transmission = carOptions[2].Text,
                    TotalPrice = new Amount(WaitAndGetBySelector("priceOnCart", ApplicationSettings.TimeOut.Fast).Text),
                    PickUpDateTime = DateTime.Parse(dates[0]),
                    DropOffDateTime = DateTime.Parse(dates[1])
                };

        }

        #endregion

        #region Public Members

        /// <summary>
        /// Check if results visible
        /// </summary>
        /// <returns></returns>
        public bool IsVisible()
        {
            var div = WaitAndGetBySelector("matrixHolder", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }

        /// <summary>
        /// Add car product to cart
        /// </summary>
        /// <param name="criteria">criteria object to add itinerary</param>
        /// <returns></returns>
        public Results AddToCart(SearchCriteria criteria)
        {
            try
            {
                var carType = GetUIElements("carType").Where((x, i) => i == 0 || (i % 3 == 0)).Select(x => x.Text.Split(' ')[0]).ToList();
                var selectedIndex = GetUIElements("addToCartButton").Select((x, i) => new { btn = x, index = i }).First(x => AddToCart(x.btn)).index;
                _addedItinerary.CarType = carType.ElementAt(selectedIndex);
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
