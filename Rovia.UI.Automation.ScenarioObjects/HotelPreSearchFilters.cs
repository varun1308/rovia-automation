using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class HotelPreSearchFilters:PreSearchFilters
    {
        public List<string> AdditionalPreferences { get; set; }
        public string StarRating { get; set; }
        public string HotelName { get; set; }
    }
}
