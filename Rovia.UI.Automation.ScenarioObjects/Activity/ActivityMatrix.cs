using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class ActivityMatrix : Matrix
    {
        public string Category { get; set; }
        public int ItineraryCount { get; set; }
        public Amount StaringPrice { get; set; }
    }
}
