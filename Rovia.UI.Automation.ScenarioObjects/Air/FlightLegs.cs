
using System;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class FlightLegs
    {
        public string AirportPair { get; set; }
        public int Duration { get; set; }
        public DateTime DepartTime { get; set; }
        public DateTime ArriveTime { get; set; }
        public CabinType Cabin { get; set; }
        public int Stops { get; set; }
    }
}
