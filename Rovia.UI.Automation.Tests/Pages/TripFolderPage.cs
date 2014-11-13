using System;
using System.Linq;
using System.Threading;
using AppacitiveAutomationFramework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rovia.UI.Automation.Tests.Application;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Utility;

namespace Rovia.UI.Automation.Tests.Pages
{
   public class TripFolderPage : UIPage
    {
       internal void ParseTripFolder()
       {
           //num of items in tripfolder
           var totalItems = WaitAndGetBySelector("totalItems", ApplicationSettings.TimeOut.Safe).Text;

           var pruductType = GetUIElements("pruductType").Select(x => x.GetAttribute("data-category"));

           var productName = GetUIElements("productName").Select(x=>x.Text).ToArray();
           var totalFare = GetUIElements("totalFare").Select(x => x.Text).ToArray();

           //need to split totalPassengers for count
           var totalPassengers = GetUIElements("totalPassengers").Select(x => x.Text).ToArray();
           
           //even index base fare and odd for taxes, for amount substring
           var fareBreakup = GetUIElements("fareBreakup").Select(x => x.Text).ToArray();
 
           //just added button controls remain with adding functionality
           //var modifyItemClick = GetUIElements("modifyItemClick");
           //var removeItemClick = GetUIElements("removeItemClick");
           //var continueShopping = GetUIElements("continueShopping");
           //var checkoutButton = GetUIElements("checkoutButton");

       }

       internal bool Checkout()
       {
           var btncheckout = WaitAndGetBySelector("tfCheckout", ApplicationSettings.TimeOut.Slow);
           if (btncheckout != null && btncheckout.Displayed)
           {
               btncheckout.Click();
               return true;
           }
           else
           {
               return false;
           }
       }
    }
}
