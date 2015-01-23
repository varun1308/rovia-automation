using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class AirResult:Results
    {
        public List<string> AirLines { get; set; }
        public List<FlightLeg> Legs { get; set; }
        public Passengers Passengers { get; set; }
    }
}
