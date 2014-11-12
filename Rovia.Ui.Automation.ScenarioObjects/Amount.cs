using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class Amount
    {
        public double TotalAmount { get; set; }
        public double AmountPerPerson { get; set; }
        public string Description { get; set; }
        public string Currency { get; set; }
    }
}
