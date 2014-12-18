using System.Collections.Generic;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class HotelPreSearchFilters:PreSearchFilters
    {
        public List<string> AdditionalPreferences { get; set; }
        public string StarRating { get; set; }
        public string HotelName { get; set; }
    }
}
