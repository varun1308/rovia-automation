using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Criteria
{
    public class HotelSearchCriteria:SearchCriteria
    {
        public string Location { get; set; }
        public StayPeriod StayPeriod { get; set; }
    }
}
