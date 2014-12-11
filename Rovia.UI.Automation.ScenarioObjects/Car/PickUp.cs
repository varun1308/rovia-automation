using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rovia.Ui.Automation.ScenarioObjects
{
    public class PickUp
    {
        public PickUpType PickUpType { get; set; }
        public string PickUpLocCode { get; set; }
        public string PickUpLocation { get; set; }
        public DateTime PickUpTime { get; set; }
    }
}
