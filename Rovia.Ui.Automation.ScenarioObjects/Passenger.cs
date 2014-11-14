﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rovia.Ui.Automation.ScenarioObjects
{
    public abstract class Passenger
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Emailid { get; set; }
        public string Gender { get; set; }
    }
}