using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public class CarResultHolder : UIPage, IResultsHolder
    {
        private static Dictionary<CarResult, IUIWebElement> _results;
        private CarResult _addedItinerary ;

        private bool AddToCart(IUIWebElement btnAddToCart)
        {

            btnAddToCart.Click();
            var divloader = WaitAndGetBySelector("divLoader", ApplicationSettings.TimeOut.Fast);
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
            return new CarResult()
                {
                    RentalAgency =
                    WaitAndGetBySelector("rentalAgencyOnCArt", ApplicationSettings.TimeOut.Fast).GetAttribute("alt"),
                    AirConditioning = carOptions[1].Text,
                    Transmission = carOptions[2].Text,
                    TotalPrice =new Amount(WaitAndGetBySelector("priceOnCart", ApplicationSettings.TimeOut.Fast).Text)
                };

        }

        public bool IsVisible()
        {
            var div = WaitAndGetBySelector("matrixHolder", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }

        public List<Results> ParseResults()
        {
            var rentalAgency = GetUIElements("rentalAgency").Select(x => x.GetAttribute("alt")).ToArray();
            var carDetails = GetUIElements("carType").ToArray();
            var carType = new string[carDetails.Length / 3];
            var j = 0; var i = 0;
            for (j = 0; i < carDetails.Length; i = i + 3, j++)
                carType[j] = carDetails[i].Text.Split(' ')[0];

            var carOptions = GetUIElements("carOptions").ToArray();
            var airconditioning = new string[carOptions.Length / 4];
            for (i = 1, j = 0; i < carOptions.Length; i = i + 4, j++)
                airconditioning[j] = carOptions[i].Text;

            var transmission = new string[carOptions.Length / 4];
            for (i = 2, j = 0; i < carOptions.Length; i = i + 4, j++)
                transmission[j] = carOptions[i].Text;

            var price = GetUIElements("price");
            var pricePerWeek = price.Select(x => x.Text).Where((item, index) => index % 2 == 0).ToArray();
            var totalPrice = price.Select(x => x.Text).Where((item, index) => index % 2 != 0).ToArray();
            var btnAddToCart = GetUIElements("addToCartButton").ToArray();

            _results = new Dictionary<CarResult, IUIWebElement>();

            i = 0;
            while (i < rentalAgency.Length)
            {
                _results.Add(new CarResult()
                {
                    RentalAgency = rentalAgency[i],
                    CarType = carType[i],
                    AirConditioning = airconditioning[i],
                    Transmission = transmission[i],
                    PricePerWeek = new Amount(pricePerWeek[i]),
                    TotalPrice = new Amount(totalPrice[i])
                },btnAddToCart[i]);
                i++;
            }
            return new List<Results>(_results.Select(d => d.Key).ToList());
        }

        public Results AddToCart(string supplier)
        {
            try
            {
                var carResult = _results.First(x => AddToCart(x.Value)).Key;
                carResult.TotalPrice = _addedItinerary.TotalPrice;
                carResult.RentalAgency = _addedItinerary.RentalAgency;
                carResult.AirConditioning = _addedItinerary.AirConditioning;
                carResult.Transmission = _addedItinerary.Transmission;
                return carResult;
            }
            catch (System.InvalidOperationException)
            {
                LogManager.GetInstance().LogWarning("No suitable results found on this page");
                return null;
            }
        }
    }
}
