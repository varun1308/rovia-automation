using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Tests.Utility
{
    public static class UtilityFunctions
    {
        public static TripProductType ToTripProductType(this string productType)
        {
            switch (productType.ToLower())
            {
                case "air":
                case "flight":
                    return TripProductType.Air;
                case "car":
                    return TripProductType.Car;
                default:
                    throw new InvalidInputException(productType+" To UtilityFunctions.ToTripPRoductType");

            }
        }

        public static CabinType ToCabinType(this string cabinType)
        {
            switch (cabinType.ToLower())
            {
                case "economy (r)":
                case "economy":
                    return CabinType.Economy;
                case "premium_economy":
                case "premium economy":
                case "premiumeconomy":
                    return CabinType.Premium_Economy;
                case "first":
                case "firstclass":
                case "first class":
                    return CabinType.First;
                case "business":
                    return CabinType.Business;
                default:
                    throw new InvalidInputException(cabinType + " To UtilityFunctions.ToCabinType");

            }
        }

        public static TripProduct GetTripProduct(string tripProductType)
        {
            switch (tripProductType.ToTripProductType())
            {
                case TripProductType.Air:
                    return new AirTripProduct();
                    case TripProductType.Car:
                    return new CarTripProduct();
                default:
                    throw new InvalidInputException(tripProductType + " To UtilityFunctions.GetTripProduct");

            }
        }
    }
}
