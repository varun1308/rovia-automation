using System;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class DropOff
    {
        public DropOffType DropOffType { get; set; }
        public string DropOffLocCode { get; set; }
        public string DropOffLocation { get; set; }
        public DateTime DropOffTime { get; set; }
    }
}
