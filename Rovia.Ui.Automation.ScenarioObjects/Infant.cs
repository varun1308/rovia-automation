using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rovia.Ui.Automation.ScenarioObjects
{
    class Infant:Passenger
    {
        public Infant()
        {
            Age = (new Random()).Next(3);
        }
    }
}
