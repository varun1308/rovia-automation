using System;
using System.Collections.Generic;
using System.Linq;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Utility;


namespace Rovia.UI.Automation.Tests.Pages
{
    public class TripFolderPage : UIPage
    {
        private IEnumerable<Fare> ParseFares()
        {
            var fares = new List<Fare>();
            var i = 0;
            var totalFare = GetUIElements("totalFare").Select(x => x.Text).ToArray();
            var baseFare =
                GetUIElements("fareBreakup")
                   .Where((item) => item.Text.Contains("Base "))
                    .Select(x => x.Text.Split(':')[1].TrimStart(' '))
                    .ToArray();
            var taxes =
                GetUIElements("fareBreakup")
                    .Where((item) => item.Text.Contains("Taxes & fees"))
                    .Select(x => x.Text.Split(':')[1].TrimStart(' '))
                    .ToArray();

            while (i < totalFare.Length)
            {
               fares.Add(new Fare()
               {
                   TotalFare = totalFare[i],
                   BaseFare = baseFare[i],
                   Taxes = taxes[i]
               });
                i++;
            }
            return fares;
        }

        private IEnumerable<Passengers> ParsePassengers()
        {
            return GetUIElements("totalPassengers").Select(x =>
            {

                var passengers = x.Text.Split(new[] { ' ', ',' }).ToList();
                return new Passengers()
                {
                    Adults = passengers.Contains("Adult") ? int.Parse(passengers[passengers.IndexOf("Adult") - 1]) : 0,
                    Children = passengers.Contains("Child") ? int.Parse(passengers[passengers.IndexOf("Child") - 1]) : 0,
                    Infants =
                        passengers.Contains("Infant") ? int.Parse(passengers[passengers.IndexOf("Infant") - 1]) : 0
                };
            });
        }

        private void ParseTripProduct(ref TripProduct product)
        {
            switch (product.ProductType)
            {
                case TripProductType.Air:
                    break;
                case TripProductType.Hotel:
                    break;
                case TripProductType.Car:
                    product = ParseCarTripProduct(product as CarTripProduct);
                    break;
            }
        }

        private TripProduct ParseCarTripProduct(CarTripProduct carTripProduct)
        {
            carTripProduct.CarType = GetUIElements("productName").Select(x => x.Text.Split(' ')[0]).ToArray()[0];
            var carOptions = GetUIElements("carACnTransmission").ToArray();
            carTripProduct.AirConditioning = carOptions[1].Text;
            carTripProduct.Transmission = carOptions[2].Text;
            //to do impletentation for Rental Agency
            return carTripProduct;
        }

        private bool VerifyAmount(string totalPriceOnCart, string totalPriceOnTrip)
        {
            return totalPriceOnCart.Equals(totalPriceOnTrip);
        }

        private List<TripProduct> ParseTripProducts()
        {
            var tripProducts = new List<TripProduct>();
            var productTypes = GetUIElements("pruductType").Select(x => x.GetAttribute("data-category")).ToArray();
            var productTitle = GetUIElements("productName").Select(x => x.Text).ToArray();
            var fares = ParseFares().ToArray();
            var modifyProductButton = GetUIElements("modifyItemClick").ToArray();
            var removeProductButton = GetUIElements("removeItemClick").ToArray();
            var passengers = ParsePassengers().ToArray();
            var i = 0;
            while (i < productTypes.Length)
            {
                var product = UtilityFunctions.GetTripProduct(productTypes[i]);
                ParseTripProduct(ref product);
                product.ProductTitle = productTitle[i];
                product.Fares = fares[i];
                product.Passengers = passengers.Length > 0 ? passengers[i] : null;
                product.ModifyProductButton = modifyProductButton[i];
                product.RemoveProductButton = removeProductButton[i];

                tripProducts.Add(product);
                i++;
            }
            return tripProducts;
        }

        internal void EditTripName()
        {
            WaitAndGetBySelector("editTripName", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("tripNameTextbox", ApplicationSettings.TimeOut.Slow).SendKeys("EditedTripName");

            //if wants to test cancel button/closing popup funcionality uncomment below & comment savetrip click 
            //WaitAndGetBySelector("cancelEditTripName",ApplicationSettings.TimeOut.Fast).Click();

            WaitAndGetBySelector("SaveTripName", ApplicationSettings.TimeOut.Fast).Click();
        }

        internal bool VerifyAddedItinerary(Results addedItem, TripFolder tripFolder)
        {
            var i = 0;
            while (i < tripFolder.TripProducts.Count)
            {
                switch (tripFolder.TripProducts[i].ProductType)
                {
                    case TripProductType.Air:
                        break;
                    case TripProductType.Hotel:
                        break;
                    case TripProductType.Car:
                        var car = addedItem as CarResult;
                        var carTrip = tripFolder.TripProducts[i] as CarTripProduct;
                        return car.TotalPrice.Equals(tripFolder.TripProducts[i].Fares.TotalFare) &&
                            car.CarType.Equals(carTrip.CarType)&&car.AirConditioning.Equals(carTrip.AirConditioning)&&
                            car.Transmission.Equals(carTrip.Transmission);
                    case TripProductType.Activity:
                        break;
                }
                i++;
            }
            return true;
        }

        internal TripFolder ParseTripFolder()
        {
            TripFolder trip = new TripFolder()
            {
                TotalTripProducts =
                    Convert.ToInt32(WaitAndGetBySelector("totalItems", ApplicationSettings.TimeOut.Safe).Text),
                TripProducts = ParseTripProducts(),
                ContinueShoppingButton = WaitAndGetBySelector("continueShopping", ApplicationSettings.TimeOut.Fast),
                CheckoutTripButton = WaitAndGetBySelector("checkoutButton", ApplicationSettings.TimeOut.Slow),
                TripSettingsButton = WaitAndGetBySelector("tripSettings", ApplicationSettings.TimeOut.Fast),
                SaveTripButton = WaitAndGetBySelector("saveTripButton", ApplicationSettings.TimeOut.Fast),
                StartoverButton = WaitAndGetBySelector("startOver", ApplicationSettings.TimeOut.Fast)
            };
            return trip;
        }
    }
}
