using System.Collections.Generic;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class Fare
    {
        public Amount TotalFare { get; set; }
        public Amount BaseFare { get; set; }
        public Amount Taxes { get; set; }
    }
}
