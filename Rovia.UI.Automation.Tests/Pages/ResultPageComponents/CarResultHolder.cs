using System;
using System.Collections.Generic;
using System.Linq;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents
{
  public class CarResultHolder:UIPage,IResultsHolder
    {
        public bool IsVisible()
        {
            var div = WaitAndGetBySelector("matrixHolder", ApplicationSettings.TimeOut.Slow);
            return div != null && div.Displayed;
        }

        public List<Results> ParseResults()
        {
            var rentalAgency = GetUIElements("rentalAgency").Select(x=>x.GetAttribute("alt")).ToArray(); 
            var carType = GetUIElements("carType").Take(1).Skip(2).Select(x => x.Text).ToArray();
            var carOptions = GetUIElements("carOptions");
            var airconditioning = carOptions.Skip(1).Take(1).Skip(2).Select(x => x.Text).ToArray();
            var transmission = carOptions.Skip(2).Take(1).Skip(1).Select(x => x.Text).ToArray();
            var price = GetUIElements("price");
            var pricePerWeek = price.Select(x => x.Text).Where((item, index) => index % 2 == 0).ToArray();
            var totalPrice = price.Select(x => x.Text).Where((item, index) => index % 2 != 0).ToArray();
            var btnAddToCart = GetUIElements("addToCartButton");
            return null;
        }

        public Results AddToCart(string result)
        {
            throw new NotImplementedException();
        }
    }
}
