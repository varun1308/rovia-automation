using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rovia.UI.Automation.ScenarioObjects.Air
{
    public class FlightSegment
    {
        public FlightSegment(FlightSegment original)
        {
            AirportPair = new AirportPair(original.AirportPair);
            AirLine = original.AirLine;
            Duration = original.Duration;
        }

        public FlightSegment()
        {
        }

        public AirportPair AirportPair { get; set; }

        public string AirLine { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
