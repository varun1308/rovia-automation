using System.Text;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Pages;

namespace Rovia.UI.Automation.Tests.Validators
{
    public static class AirValidator
    {
        public static void ValidateTripProduct(this TripFolderPage page, AirTripProduct airTripProduct,
                                                    AirResult airResult)
        {
            LogManager.GetInstance().LogDebug("Validating Air trip product on TripFolder Page");

            var errors = new StringBuilder();
            if (!airResult.Amount.Equals(airTripProduct.Fares.TotalFare))
                errors.Append(FormatError("Amount", airResult.Amount.ToString(), airTripProduct.Fares.TotalFare.ToString()));

            if (airTripProduct.FlightLegs.Count == airResult.Legs.Count)
            {
                for (var i = 0; i < airTripProduct.FlightLegs.Count; i++)
                {
                    if (!airTripProduct.FlightLegs[i].AirportPair.Equals(airResult.Legs[i].AirportPair))
                        errors.Append(FormatError("AirportPair", airResult.Legs[i].AirportPair, airTripProduct.FlightLegs[i].AirportPair));
                    if (!airTripProduct.FlightLegs[i].ArriveTime.Equals(airResult.Legs[i].ArriveTime))
                        errors.Append(FormatError("Arrival Time", airResult.Legs[i].ArriveTime.ToLongDateString(), airTripProduct.FlightLegs[i].ArriveTime.ToLongDateString()));
                    if (!airTripProduct.FlightLegs[i].DepartTime.Equals(airResult.Legs[i].DepartTime))
                        errors.Append(FormatError("Depart Time", airResult.Legs[i].DepartTime.ToLongDateString(), airTripProduct.FlightLegs[i].DepartTime.ToLongDateString()));
                    //need to parsing on result page first
                    //if (!airTripProduct.FlightLegs[i].Cabin.Equals(airResult.Legs[i].Cabin))
                    //    errors.Append(FormatError("Cabin", airResult.Legs[i].Cabin.ToString(), airTripProduct.FlightLegs[i].Cabin.ToString()));
                    //if (airTripProduct.FlightLegs[i].Duration != airResult.Legs[i].Duration && WaitAndGetBySelector("changeFlightTimes", ApplicationSettings.TimeOut.Fast) == null)
                    //    errors.Append(FormatError("Duration", airResult.Legs[i].Duration.ToString(), airTripProduct.FlightLegs[i].Duration.ToString()));
                }
            }
            else
                errors.Append(FormatError("Leg counts ", airResult.Legs.Count.ToString(), airTripProduct.FlightLegs.Count.ToString()));

            if (!airResult.AirLines.TrueForAll(airTripProduct.Airlines.Contains))
                errors.Append(FormatError("Airlines", string.Join(",", airResult.AirLines.ToArray()), string.Join(",", airTripProduct.Airlines.ToArray())));

            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on TripFolderPage");
        }

        public static void ValidateTripProduct(this PassengerInfoPage page, AirTripProduct airTripProduct, AirResult airResult)
        {
            LogManager.GetInstance().LogDebug("Validating Air trip product on PassengerInfo Page");
            var errors = new StringBuilder();
            if (!airResult.Amount.Equals(airTripProduct.Fares.TotalFare))
                errors.Append(FormatError("Amount", airResult.Amount.ToString(), airTripProduct.Fares.TotalFare.ToString()));

            if (airTripProduct.FlightLegs.Count == airResult.Legs.Count)
            {
                for (var i = 0; i < airTripProduct.FlightLegs.Count; i++)
                {
                    if (!airTripProduct.FlightLegs[i].AirportPair.Equals(airResult.Legs[i].AirportPair))
                        errors.Append(FormatError("AirportPair", airTripProduct.FlightLegs[i].AirportPair, airResult.Legs[i].AirportPair));
                    if (!airTripProduct.FlightLegs[i].ArriveTime.Equals(airResult.Legs[i].ArriveTime))
                        errors.Append(FormatError("Arrival Time", airTripProduct.FlightLegs[i].ArriveTime.ToLongDateString(), airResult.Legs[i].ArriveTime.ToLongDateString()));
                    if (!airTripProduct.FlightLegs[i].DepartTime.Equals(airResult.Legs[i].DepartTime))
                        errors.Append(FormatError("Depart Time", airTripProduct.FlightLegs[i].DepartTime.ToLongDateString(), airResult.Legs[i].DepartTime.ToLongDateString()));
                }
            }
            else
                errors.Append(FormatError("Leg counts ", airTripProduct.FlightLegs.Count.ToString(), airResult.Legs.Count.ToString()));

            if (!airResult.AirLines.TrueForAll(airTripProduct.Airlines.Contains))
                errors.Append(FormatError("Airlines", string.Join(",", airResult.AirLines.ToArray()), string.Join(",", airTripProduct.Airlines.ToArray())));

            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on PassengerInfoPage");
        }

        public static void ValidateTripProduct(this CheckoutPage page, AirTripProduct airTripProduct, AirResult airResult)
        {
            LogManager.GetInstance().LogDebug("Validating Air trip product on Checkout Page");
            var errors = new StringBuilder();
            if (!airResult.Amount.Equals(airTripProduct.Fares.TotalFare))
                errors.Append(FormatError("Amount", airResult.Amount.ToString(), airTripProduct.Fares.TotalFare.ToString()));

            if (airTripProduct.FlightLegs.Count == airResult.Legs.Count)
            {
                for (var i = 0; i < airTripProduct.FlightLegs.Count; i++)
                {
                    if (!airTripProduct.FlightLegs[i].AirportPair.Equals(airResult.Legs[i].AirportPair))
                        errors.Append(FormatError("AirportPair", airTripProduct.FlightLegs[i].AirportPair, airResult.Legs[i].AirportPair));
                    if (!airTripProduct.FlightLegs[i].ArriveTime.Equals(airResult.Legs[i].ArriveTime))
                        errors.Append(FormatError("Arrival Time", airTripProduct.FlightLegs[i].ArriveTime.ToLongDateString(), airResult.Legs[i].ArriveTime.ToLongDateString()));
                    if (!airTripProduct.FlightLegs[i].DepartTime.Equals(airResult.Legs[i].DepartTime))
                        errors.Append(FormatError("Depart Time", airTripProduct.FlightLegs[i].DepartTime.ToLongDateString(), airResult.Legs[i].DepartTime.ToLongDateString()));
                }
            }
            else
                errors.Append(FormatError("Leg counts ", airTripProduct.FlightLegs.Count.ToString(), airResult.Legs.Count.ToString()));

            if (!airResult.AirLines.TrueForAll(airTripProduct.Airlines.Contains))
                errors.Append(FormatError("Airlines", string.Join(",", airResult.AirLines.ToArray()), string.Join(",", airTripProduct.Airlines.ToArray())));

            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on CheckoutPage");
        }

        public static void ValidateBookedTripProducts(this CheckoutPage page, AirTripProduct airTripProduct, AirResult airResult)
        {
            //Todo Implement Confirmation Page Validation
        }
        
        private static string FormatError(string error, string addedValue, string tfValue)
        {
            return string.Format("| Invalid {0} ({1}, {2})", error, addedValue, tfValue);
        }
    }
}
