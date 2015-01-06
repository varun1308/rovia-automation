using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.ScenarioObjects.Activity
{
    public class ActivityResult:Results
    {
        public string Name { get; set; }

        public string Category { get; set; }

        public string ProductName { get; set; }

        public DateTime Date { get; set; }

        public Passengers Passengers { get; set; }

        public string Description { get; set; }
    }
}
