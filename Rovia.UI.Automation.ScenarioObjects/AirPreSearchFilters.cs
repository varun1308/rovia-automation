using System.Collections.Generic;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class AirPreSearchFilters:PreSearchFilters
    {
        public CabinType CabinType { get; set; }
        public bool NonStopFlight { get; set; }
        public List<string> AirLines { get; set; }
        public bool IncludeNearByAirPorts { get; set; }
    }
}
