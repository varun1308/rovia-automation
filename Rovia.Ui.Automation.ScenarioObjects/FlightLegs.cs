using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class FlightLegs
    {
        public string AirportPair { get; set; }
        public int Duration { get; set; }
        public string DepartTime { get; set; }
        public string ArriveTime { get; set; }
        public CabinType Cabin { get; set; }
        public int Stops { get; set; }
    }
}
