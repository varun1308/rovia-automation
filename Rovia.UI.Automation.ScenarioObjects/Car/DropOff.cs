using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rovia.Ui.Automation.ScenarioObjects
{
    public class DropOff
    {
        public DropOffType DropOffType { get; set; }
        public string DropOffLocCode { get; set; }
        public string DropOffLocation { get; set; }
        public DateTime DropOffTime { get; set; }
    }
}
