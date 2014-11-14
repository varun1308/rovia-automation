using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.Ui.Automation.ScenarioObjects
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
