using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppacitiveAutomationFramework;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class AirPostSearchFilters : PostSearchFilters
    {
        public FlightStops FlightStops { get; set; }
        public List<IUIWebElement> Airlines { get; set; }
        public List<IUIWebElement> CabinType { get; set; }
    }
}
