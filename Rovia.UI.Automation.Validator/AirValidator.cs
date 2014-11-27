using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Criteria;

namespace Rovia.UI.Automation.Validator
{
    public class AirValidator:IValidator
    {
        public bool ValidatePreSearchFilters(PreSearchFilters preSearchFilters, Results parseResults)
        {

            //var  expectedFilters= preSearchFilters as AirPreSearchFilters;
            //var appliedFilters = parseResults as AirResult;

            //if (expectedFilters != null && appliedFilters != null)
            //{
            //    if (expectedFilters.NonStopFlight)
            //    {
            //        if(appliedFilters.Legs.Select(x => x.Stops).Any(x => x > 0)) return false;
            //    }

            //    if (appliedFilters.Legs.Select(x => x.Cabin).Any(x => x.Equals(expectedFilters.CabinType.ToString()))) return true;
                
            //}


            return true;
        }
    }
}
