using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rovia.UI.Automation.Constants;

namespace Rovia.UI.Automation.Criteria
{
    public class CarSearchCriteria: SearchCriteria
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string PickUpLocation { get; set; }
        public string DropOffLocation { get; set; }
        public CarLocationType PickUpLocationType { get; set; }
        public CarLocationType DropOffLocationType { get; set; }
        public int PickUpRadius { get; set; }
        public int DropOffRadius { get; set; }
    }
}
