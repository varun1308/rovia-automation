﻿using System;

namespace Rovia.UI.Automation.ScenarioObjects
{
    internal class Adult:Passenger
    {
        public Adult()
        {
            Age =(new Random()).Next(19, 99);
        }
    }
}