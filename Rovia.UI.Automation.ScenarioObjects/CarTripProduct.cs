﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class CarTripProduct : TripProduct
    {
        public CarTripProduct()
        {
            ProductType = TripProductType.Car;
        }
    }
}
