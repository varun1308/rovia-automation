
using System;
using System.Collections.Generic;
using Rovia.UI.Automation.ScenarioObjects.Air;
using System.Linq;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class FlightLeg
    {
        private int _stops;
        private TimeSpan _duration;
        public AirportPair AirportPair { get; set; }
        public TimeSpan Duration {
            get
            {
                if (_duration != TimeSpan.Zero)
                    return _duration;
                var duration = TimeSpan.Zero;
                Segments.ForEach(x=>duration+=x.Duration);
                LayOvers.ForEach(x=>duration+=x);
                return duration;
            }
            set { _duration = value; }
        }
        public CabinType Cabin { get; set; }
        public int Stops {
            get
            {
                if (_stops == 0 && LayOvers != null)
                    return LayOvers.Count;
                return _stops;
            }
            set
            {
                _stops = value;
            }
        }
        public List<TimeSpan> LayOvers { get; set; }
        public List<FlightSegment> Segments { get; set; }
    }
}
