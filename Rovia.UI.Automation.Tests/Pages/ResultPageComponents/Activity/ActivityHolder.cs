using System;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Criteria;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.ScenarioObjects.Activity;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents.Activity
{
    public class ActivityHolder : UIPage
    {
        private ActivityResult _addedResult;
        internal void WaitForLoad()
        {
            try
            {
                IUIWebElement webElement;
                do
                {
                    webElement = WaitAndGetBySelector("activityDetailsHolder", ApplicationSettings.TimeOut.Slow);
                } while (webElement == null || !webElement.Displayed);
                webElement = WaitAndGetBySelector("alerts", ApplicationSettings.TimeOut.Fast);
                if (webElement != null && webElement.Displayed)
                    throw new Alert(webElement.Text);
            }
            catch (Exception exception)
            {
                throw new PageLoadFailed("HotelRoomPage", exception);
            }
        }

        internal Results AddToCart(ActivitySearchCriteria criteria)
        {

            var description = WaitAndGetBySelector("activityDescription", ApplicationSettings.TimeOut.Fast).Text;
            var category = WaitAndGetBySelector("activityCategory", ApplicationSettings.TimeOut.Fast).Text.Split(':')[1].Trim();
            var i = 1;
            var productBars = GetUIElements("productBar");
            var btnAddAdult = GetUIElements("adultCount").Where((x, index) => index % 2 == 1).ToArray();
            var activityName = WaitAndGetBySelector("activityTitle", ApplicationSettings.TimeOut.Fast).Text;
            if (!btnAddAdult.Any())
                criteria.Passengers.Adults = 0;
            else
                criteria.AdultAgeGroup = new AgeGroup(GetUIElements("adultAgeGrp")[0].Text);
            var btnAddChildren = GetUIElements("childrenCount").Where((x, index) => index % 2 == 1).ToArray();
            
            if (!btnAddChildren.Any())
                criteria.Passengers.Children = 0;
            else
                criteria.ChildrenAgeGroup = new AgeGroup(GetUIElements("childrenAgeGrp")[0].Text);
            var btnAddInfant = GetUIElements("infantCount").Where((x, index) => index % 2 == 1).ToArray();

            if (!btnAddInfant.Any())
                criteria.Passengers.Infants = 0;
            else
                criteria.InfantAgeGroup = new AgeGroup(GetUIElements("infantAgeGrp")[0].Text);

            foreach (var btn in GetUIElements("btnAddToCart"))
            {
                for (var j = 1; j < criteria.Passengers.Adults; j++)
                    btnAddAdult[i - 1].Click();
                for (var j = 0; j < criteria.Passengers.Children; j++)
                    btnAddChildren[i - 1].Click();
                for (var j = 0; j < criteria.Passengers.Infants; j++)
                    btnAddInfant[i - 1].Click();
                if (!AddToCart(btn))
                    if (i < productBars.Count)
                        productBars[i++].Click();
                    else
                    {
                        LogManager.GetInstance().LogWarning("No Activity Could Be Added");
                        throw new AddToCartFailedException();
                    }
                else
                {
                    _addedResult.Name = activityName;
                    _addedResult.Description = description;
                    _addedResult.Passengers = criteria.Passengers;
                    _addedResult.Category = category;
                    return _addedResult;
                }
            }
            return null;
        }

        private bool AddToCart(IUIWebElement btnAddToCart)
        {
            btnAddToCart.Click();
            //var divloader = WaitAndGetBySelector("divLoader", ApplicationSettings.TimeOut.Fast);
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
                _addedResult = ParseResultFromCart();
                btnCheckOut.Click();
                return true;
            }
            WaitAndGetBySelector("btnCancel", ApplicationSettings.TimeOut.Slow).Click();
            return false;
        }

        private ActivityResult ParseResultFromCart()
        {
            var activityDetails = GetUIElements("activityDetails").Select(x => x.Text).ToArray();
            return new ActivityResult()
                {
                    ProductName = activityDetails[1],
                    Date = DateTime.Parse(WaitAndGetBySelector("activityDate", ApplicationSettings.TimeOut.Fast).Text.Remove(0, 5).Trim()),
                    Amount = new Amount(WaitAndGetBySelector("price", ApplicationSettings.TimeOut.Fast).Text)
                };
        }
    }
}
