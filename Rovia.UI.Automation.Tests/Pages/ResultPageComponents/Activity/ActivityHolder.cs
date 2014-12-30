using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;

namespace Rovia.UI.Automation.Tests.Pages.ResultPageComponents.Activity
{
    public class ActivityHolder : UIPage
    {
        private HotelResult _addedResult;
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

        internal Results AddToCart()
        {
            var i = 1;
            var productBars = GetUIElements("productBar");
            foreach (var btn in GetUIElements("btnAddToCart"))
            {
                if (!AddToCart(btn))
                    if (i < productBars.Count)
                        productBars[i++].Click();
                    else
                    {
                        LogManager.GetInstance().LogWarning("No Room Could Be Added");
                        throw new AddToCartFailedException();
                    }
                else
                    return _addedResult;
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

        private HotelResult ParseResultFromCart()
        {
            return null;
        }
    }
}
