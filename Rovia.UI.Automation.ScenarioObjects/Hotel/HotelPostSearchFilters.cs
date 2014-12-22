using System;
using System.Collections.Generic;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class HotelPostSearchFilters : PostSearchFilters
    {
        public string HotelName { get; set; }
        public RatingRange RatingRange { get; set; }

        public List<string> Amenities { get; set; }

        public Tuple<string,string> PreferredLocation { get; set; }

        public DistanceRange DistanceRange { get; set; }
    }
}
