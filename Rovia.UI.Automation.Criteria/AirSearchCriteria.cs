using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Criteria
{
    public class AirSearchCriteria: SearchCriteria
    {
        public List<AirportPair> AirportPairs { get; set; }
        public SearchType SearchType { get; set; }
        public Passengers Passengers { get; set; }
        public AirFilters Filters { get; set; }
    }
}
