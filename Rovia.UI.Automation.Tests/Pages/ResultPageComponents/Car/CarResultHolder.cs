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

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
    public class CarResultHolder : UIPage, IResultsHolder
    {
        private CarResult _addedItinerary;

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

        public bool IsVisible()
        {
            var div = WaitAndGetBySelector("matrixHolder", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }

        public List<Results> ParseResults()
        {
            var rentalAgency = GetUIElements("rentalAgency").Select(x => x.GetAttribute("alt"));

            var carType = GetUIElements("carType").Where((x, index) => index == 0 || (index % 3 == 0)).Select(x => x.Text.Split(' ')[0]).ToList();
            var carOptions = GetUIElements("carOptions");
            var airconditioning = carOptions.Skip(1).Where((x, index) => index % 4 == 0).Select(x => x.Text).ToList();
            var transmission = carOptions.Skip(2).Where((x, index) => index % 4 == 0).Select(x => x.Text).ToList();

            var price = GetUIElements("price");
            var pricePerWeek = price.Select(x => x.Text).Where((item, index) => index % 2 == 0).ToArray();
            var totalPrice = price.Select(x => x.Text).Where((item, index) => index % 2 != 0).ToArray();
            var locations = GetUIElements("locations").Select(x => x.Text.Split('-')[0].TrimEnd(' ')).Where((item, index) => index % 2 == 0).ToArray();

            var allDateTimes = GetUIElements("DateTimes").Where((item) => item.Text.Contains(" AM") || item.Text.Contains(" PM")).Select(x => x.Text).ToList();
            var pickUpDateTimes = allDateTimes.Where(((item, index) => index % 2 == 0)).ToArray();
            var dropOffDateTimes = allDateTimes.Where(((item, index) => index % 2 != 0)).ToArray();

            return rentalAgency.Select((x, index) => new CarResult()
                {
                    RentalAgency = x,
                    CarType = carType[index],
                    AirConditioning = airconditioning[index],
                    Transmission = transmission[index],
                    PricePerWeek = new Amount(pricePerWeek[index]),
                    TotalPrice = new Amount(totalPrice[index]),
                    Location = locations[index],
                    PickUpDateTime = DateTime.Parse(pickUpDateTimes[index]),
                    DropOffDateTime = DateTime.Parse(dropOffDateTimes[index])
                } as Results).ToList();

        }

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
    }
}
