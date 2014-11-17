using System.Collections.Generic;
using AppacitiveAutomationFramework;

namespace Rovia.UI.Automation.ScenarioObjects
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
