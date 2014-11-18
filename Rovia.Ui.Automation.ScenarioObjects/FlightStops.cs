using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppacitiveAutomationFramework;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class FlightStops
    {
        public IUIWebElement NonStop { get; set; }
        public IUIWebElement OneStop { get; set; }
        public IUIWebElement OnePlusStop { get; set; }
    }
}
