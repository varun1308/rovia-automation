namespace Rovia.UI.Automation.Criteria
{
    using System.Collections.Generic;
    using ScenarioObjects;

    public class AirSearchCriteria: SearchCriteria
    {
        public List<AirportPair> AirportPairs { get; set; }
        public SearchType SearchType { get; set; }
    }
}
