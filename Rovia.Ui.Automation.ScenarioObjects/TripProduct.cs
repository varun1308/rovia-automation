
using AppacitiveAutomationFramework;

namespace Rovia.UI.Automation.ScenarioObjects
{
   public abstract class TripProduct
    {
        public TripProductType ProductType { get; protected set; }
        public string ProductTitle { get; set; }
        public Fare Fares { get; set; }
        public Passengers Passengers { get; set; }
        public IUIWebElement ModifyProductButton { get; set; }
        public IUIWebElement RemoveProductButton { get; set; }
    }
}
