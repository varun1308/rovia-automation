using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rovia.Ui.Automation.ScenarioObjects
{
    internal class Adult:Passenger
    {
        public Adult()
        {
            Age =(new Random()).Next(18, 99);
        }
    }
}
