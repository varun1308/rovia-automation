using System.Collections.Generic;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Criteria
{
    public class AirSearchCriteria: SearchCriteria
    {
        public List<AirportPair> AirportPairs { get; set; }
        public SearchType SearchType { get; set; }
        public AirPreSearchFilters PreSearchFiltersFilters { get; set; }
    }
}
