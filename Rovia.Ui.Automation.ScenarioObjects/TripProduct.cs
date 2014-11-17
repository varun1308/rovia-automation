
using AppacitiveAutomationFramework;

namespace Rovia.UI.Automation.ScenarioObjects
{
   public class TripProduct
    {
        public string ProductType { get; set; }
        public string ProductTitle { get; set; }
        public Fare Fares { get; set; }
        public Passengers Passengers { get; set; }
        public IUIWebElement ModifyProductButton { get; set; }
        public IUIWebElement RemoveProductButton { get; set; }
    }
}
