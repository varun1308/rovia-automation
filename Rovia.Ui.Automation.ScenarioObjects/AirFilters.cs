using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class AirFilters
    {
        public CabinType CabinType { get; set; }
        public bool NonStopFlight { get; set; }
        public List<string> AirLines { get; set; }
        public bool IncludeNearByAirPorts { get; set; }
    }
}
