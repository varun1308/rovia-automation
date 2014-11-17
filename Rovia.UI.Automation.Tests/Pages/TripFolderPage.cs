using System;
using System.Collections.Generic;
using System.Linq;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages
{
    public class TripFolderPage : UIPage
    {
        internal TripFolder ParseTripFolder()
        {
            TripFolder trip = new TripFolder()
            {
                TotalTripProducts =
                    Convert.ToInt32(WaitAndGetBySelector("totalItems", ApplicationSettings.TimeOut.Safe).Text),
                TripProducts = new List<TripProduct>(),
                ContinueShoppingButton = WaitAndGetBySelector("continueShopping", ApplicationSettings.TimeOut.Fast),
                CheckoutTripButton = WaitAndGetBySelector("checkoutButton", ApplicationSettings.TimeOut.Fast),
                TripSettingsButton = WaitAndGetBySelector("tripSettings", ApplicationSettings.TimeOut.Fast),
                SaveTripButton = WaitAndGetBySelector("saveTripButton", ApplicationSettings.TimeOut.Fast),
                StartoverButton = WaitAndGetBySelector("startOver", ApplicationSettings.TimeOut.Fast)
            };
            var productTypes = GetUIElements("pruductType").Select(x => x.GetAttribute("data-category")).ToArray();
            var productTitle = GetUIElements("productName").Select(x => x.Text).ToArray();
            var fares = ParseFares().ToArray();
            var modifyProductButton = GetUIElements("modifyItemClick").ToArray();
            var removeProductButton = GetUIElements("removeItemClick").ToArray();
            var passengers = ParsePassengers().ToArray();
            var i = 0;
            while (i < trip.TotalTripProducts)
            {
                trip.TripProducts.Add(new TripProduct()
                {
                    ProductType = productTypes[i],
                    ProductTitle = productTitle[i],
                    Fares = fares[i],
                    Passengers = passengers[i],
                    ModifyProductButton = modifyProductButton[i],
                    RemoveProductButton = removeProductButton[i]

                });
                i++;
            }
            return trip;
        }

        private IEnumerable<Fare> ParseFares()
        {
            return new List<Fare>()
            {
                new Fare()
                {
                    TotalFare = GetUIElements("totalFare").Select(x => x.Text).ToList(),
                    BaseFare =
                        GetUIElements("fareBreakup")
                            .Where((item, index) => index%2 == 0)
                            .Select(x => x.Text.Split(':')[1].TrimStart(' '))
                            .ToList(),
                    Taxes =
                        GetUIElements("fareBreakup")
                            .Where((item, index) => index%2 != 0)
                            .Select(x => x.Text.Split(':')[1].TrimStart(' '))
                            .ToList()
                }
            };
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


        internal void EditTripName()
        {
            WaitAndGetBySelector("editTripName", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("tripNameTextbox", ApplicationSettings.TimeOut.Slow).SendKeys("EditedTripName");

            //if wants to test cancel button/closing popup funcionality uncomment below & comment savetrip click 
            //WaitAndGetBySelector("cancelEditTripName",ApplicationSettings.TimeOut.Fast).Click();

            WaitAndGetBySelector("SaveTripName", ApplicationSettings.TimeOut.Fast).Click();
        }
    }
}
