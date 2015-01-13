using System;
using System.Collections.Generic;
using AppacitiveAutomationFramework;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class AirPostSearchFilters : PostSearchFilters
    {
        
        public TakeOffTimeRange TakeOffTimeRange { get; set; }
        public LandingTimeRange LandingTimeRange { get; set; }
        public List<string> Stop { get; set; }
        public int MaxTimeDurationDiff { get; set; }
        public List<CabinType> CabinTypes { get; set; }
        public List<string> Airlines { get; set; }
    }
}
