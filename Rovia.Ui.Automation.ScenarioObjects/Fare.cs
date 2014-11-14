using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rovia.Ui.Automation.ScenarioObjects
{
    public class Fare
    {
        public List<string> TotalFare { get; set; }
        public List<string> BaseFare { get; set; }
        public List<string> Taxes { get; set; }
    }
}
