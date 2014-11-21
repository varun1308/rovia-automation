using System;
using System.Collections.Generic;
using AppacitiveAutomationFramework;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class AirPostSearchFilters : PostSearchFilters
    {
        public bool IsApplyFilter { get; set; }
        public PriceRange PriceRange { get; set; }
        public TakeOffTimeRange TakeOffTimeRange { get; set; }
        public LandingTimeRange LandingTimeRange { get; set; }
        public string Stop { get; set; }
        public int MaxTimeDurationDiff { get; set; }
        public List<string> CabinTypes { get; set; }
        public List<string> Airlines { get; set; }
    }
}
