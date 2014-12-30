using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.Criteria
{
    public class ActivitySearchCriteria:SearchCriteria
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string ShortLocation { get; set; }
        public string Location { get; set; }
    }
}
