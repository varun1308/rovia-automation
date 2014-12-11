using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rovia.Ui.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Criteria
{
    public class CarSearchCriteria : SearchCriteria
    {
        public PickUp PickUp { get; set; }
        public DropOff DropOff { get; set; }
    }
}
