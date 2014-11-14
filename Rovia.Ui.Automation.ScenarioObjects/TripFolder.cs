using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppacitiveAutomationFramework;

namespace Rovia.Ui.Automation.ScenarioObjects
{
   public class TripFolder
    {
       public int TotalTripProducts { get; set; }
       public List<TripProduct>  TripProducts { get; set; }
       public IUIWebElement ContinueShoppingButton { get; set; }
       public IUIWebElement CheckoutTripButton { get; set; }
       public IUIWebElement TripSettingsButton { get; set; }
       public IUIWebElement SaveTripButton { get; set; }
       public IUIWebElement StartoverButton { get; set; }
    }
}
