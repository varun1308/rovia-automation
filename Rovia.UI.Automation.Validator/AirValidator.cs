using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Criteria;

namespace Rovia.UI.Automation.Validator
{
    public class AirValidator : IValidator
    {
        #region private members
        private  bool ValidateStops(IEnumerable<AirResult> filters)
        {
            return !filters.Any(x=>x.Legs.Select(y => y.Stops).Any(z => z > 0));
        }

        private  bool ValidateCabin(AirPreSearchFilters filters, IEnumerable<AirResult> results)
        {
            return results.Any(z=>z.Legs.Select(x => x.Cabin).Any(x => x.Equals(filters.CabinType)));
        }

        private  bool ValidateAirlines(IEnumerable<List<string>> appliedAirlines, List<string> expectedAirlines)
        {
             return  appliedAirlines.Any(y=>y.Any(expectedAirlines.Contains));
        }

        #endregion

        #region public members

        public bool ValidatePreSearchFilters(PreSearchFilters preSearchFilters,List<Results> parsedResults)
        {
            var filters = preSearchFilters as AirPreSearchFilters;
            var results = parsedResults.Select(x => x as AirResult);

            return (ValidateCabin(filters, results) 
                && (filters.AirLines == null || ValidateAirlines(results.Select(x=> x.AirLines), filters.AirLines))
                &&(!filters.NonStopFlight || ValidateStops(results)));
        }

        private bool ValidateMatrix(string filterAirline,IEnumerable<Results> parsedResults )
        {
            var results = parsedResults.Select(x => x as AirResult);
            return results.All(x => x.AirLines.All(y=>y.Equals(filterAirline) || y.Contains("Multiple")));
        }

        #endregion
    }
}
