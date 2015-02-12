using Rovia.UI.Automation.Framework.Pages;
using System.Text;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.Logger;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.ScenarioObjects.Air;
using System.Linq;

namespace Rovia.UI.Automation.Framework.Validators
{
    /// <summary>
    /// This class contains all the methods for air product validations
    /// </summary>
    public static class AirValidator
    {
        #region Private Members

        private static string FormatError(string error, string addedValue, string tfValue)
        {
            return string.Format("| Invalid {0} ({1}, {2})", error, addedValue, tfValue);
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Extension method to validate air trip product with added product in cart from result page
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="airTripProduct">Air trip product on trip folder page</param>
        /// <param name="airResult">Added itinerary to cart on result page</param>
        public static void ValidateTripProduct(this TripFolderPage page, AirTripProduct airTripProduct, AirResult airResult)
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
                        errors.Append(FormatError("[Leg " + i + "] AirportPair", airResult.Legs[i].AirportPair.ToString(), airTripProduct.FlightLegs[i].AirportPair.ToString()));
                    if (!airTripProduct.FlightLegs[i].Cabin.Equals(airResult.Legs[i].Cabin))
                        errors.Append(FormatError("[Leg " + i + "] Cabin", airResult.Legs[i].Cabin.ToString(), airTripProduct.FlightLegs[i].Cabin.ToString()));
                    if (airTripProduct.FlightLegs[i].Duration != airResult.Legs[i].Duration)
                        errors.Append(FormatError("[Leg " + i + "] Duration", airResult.Legs[i].Duration.ToString(), airTripProduct.FlightLegs[i].Duration.ToString()));
                    if (airTripProduct.FlightLegs[i].Stops != airResult.Legs[i].Stops)
                        errors.Append(FormatError("[Leg " + i + "] Stops", airResult.Legs[i].Stops.ToString(), airTripProduct.FlightLegs[i].Stops.ToString()));
                    if (airTripProduct.FlightLegs[i].Segments.Count == airResult.Legs[i].Segments.Count)
                    {
                        for (int j = 0; j < airTripProduct.FlightLegs[i].Segments.Count; j++)
                        {
                            var resultSegment = airTripProduct.FlightLegs[i].Segments[j];
                            var tpSegment = airResult.Legs[i].Segments[j];
                            if (!tpSegment.AirportPair.Equals(resultSegment.AirportPair))
                                errors.Append(FormatError("[Leg " + i + ": Segment " + j + "] AirportPair", resultSegment.AirportPair.ToString(), tpSegment.AirportPair.ToString()));
                            if (!resultSegment.AirLine.Equals(tpSegment.AirLine))
                                errors.Append(FormatError("[Leg " + i + ": Segment " + j + "] Airline", resultSegment.AirLine, tpSegment.AirLine));
                            if (resultSegment.Duration != tpSegment.Duration)
                                errors.Append(FormatError("[Leg " + i + ": Segment " + j + "] Duration", resultSegment.Duration.ToString(), tpSegment.Duration.ToString()));
                        }
                    }
                    else
                        errors.Append(FormatError("[Leg " + i + "] Segment-Count", airResult.Legs[i].Segments.Count.ToString(), airTripProduct.FlightLegs[i].Segments.Count.ToString()));
                }
            }
            else
                errors.Append(FormatError("Leg counts ", airResult.Legs.Count.ToString(), airTripProduct.FlightLegs.Count.ToString()));

            if (!airResult.AirLines.TrueForAll(airTripProduct.Airlines.Contains))
                errors.Append(FormatError("Airlines", string.Join(",", airResult.AirLines.ToArray()), string.Join(",", airTripProduct.Airlines.ToArray())));

            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on TripFolderPage");

        }


        /// <summary>
        /// Extension method to validate air trip product with added product in cart from result page
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="airTripProduct">Air trip product on passenger info page</param>
        /// <param name="airResult">Added itinerary to cart on result page</param>
        public static void ValidateTripProduct(this PassengerInfoPage page, AirTripProduct airTripProduct, AirResult airResult)
        {
            LogManager.GetInstance().LogDebug("Validating Air trip product on PassengerInfo Page");
            var errors = new StringBuilder();
            if (!airResult.Amount.Equals(airTripProduct.Fares.TotalFare))
                errors.Append(FormatError("Amount", airResult.Amount.ToString(), airTripProduct.Fares.TotalFare.ToString()));
            var rSegments = airResult.Legs.SelectMany(x => x.Segments.Select(y => new FlightSegment(y))).ToArray();
            foreach (var flightSegment in rSegments)
            {
                flightSegment.AirportPair.ArrivalAirport = flightSegment.AirportPair.ArrivalAirport.Split('-')[1].Trim().Split()[0];
                flightSegment.AirportPair.DepartureAirport = flightSegment.AirportPair.DepartureAirport.Split('-')[1].Trim().Split()[0];
                flightSegment.AirLine = flightSegment.AirLine.Split('-')[0].Trim();
            }
            var pfSegment = airTripProduct.FlightLegs.SelectMany(x => x.Segments).ToArray();
            if (rSegments.Length == pfSegment.Length)
            {
                for (var j = 0; j < rSegments.Length; j++)
                {
                    if (!pfSegment[j].AirportPair.Equals(rSegments[j].AirportPair))
                        errors.Append(FormatError("[Segment " + j + "] AirportPair", rSegments[j].AirportPair.ToString(), pfSegment[j].AirportPair.ToString()));
                    if (!rSegments[j].AirLine.Equals(pfSegment[j].AirLine))
                        errors.Append(FormatError("[Segment " + j + "] Airline", rSegments[j].AirLine, pfSegment[j].AirLine));
                }
            }
            else
                errors.Append(FormatError(" Segment-Count", rSegments.Length.ToString(), pfSegment.Length.ToString()));


            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on PassengerInfoPage");
        }
        
        /// <summary>
        /// Extension method to validate air trip product with added product in cart from result page
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="airTripProduct">Air trip product on checkout page</param>
        /// <param name="airResult">Added itinerary to cart on result page</param>
        public static void ValidateTripProduct(this CheckoutPage page, AirTripProduct airTripProduct, AirResult airResult)
        {
            LogManager.GetInstance().LogDebug("Validating Air trip product on CheckOut Page");
            var errors = new StringBuilder();
            if (!airResult.Amount.Equals(airTripProduct.Fares.TotalFare))
                errors.Append(FormatError("Amount", airResult.Amount.ToString(), airTripProduct.Fares.TotalFare.ToString()));
            var rSegments = airResult.Legs.SelectMany(x => x.Segments.Select(y => new FlightSegment(y))).ToArray();
            foreach (var flightSegment in rSegments)
            {
                flightSegment.AirportPair.ArrivalAirport = flightSegment.AirportPair.ArrivalAirport.Split('-')[1].Trim().Split()[0];//GetAirPortCode(flightSegment.AirportPair.ArrivalAirport);
                flightSegment.AirportPair.DepartureAirport = flightSegment.AirportPair.DepartureAirport.Split('-')[1].Trim().Split()[0];
                flightSegment.AirLine = flightSegment.AirLine.Split('-')[0].Trim();
                //GetAirPortCode(flightSegment.AirportPair.DepartureAirport);
            }
            var pfSegment = airTripProduct.FlightLegs.SelectMany(x => x.Segments).ToArray();
            if (rSegments.Length == pfSegment.Length)
            {
                for (var j = 0; j < rSegments.Length; j++)
                {
                    if (!pfSegment[j].AirportPair.Equals(rSegments[j].AirportPair))
                        errors.Append(FormatError("[Segment " + j + "] AirportPair", rSegments[j].AirportPair.ToString(), pfSegment[j].AirportPair.ToString()));
                    if (!rSegments[j].AirLine.Equals(pfSegment[j].AirLine))
                        errors.Append(FormatError("[Segment " + j + "] Airline", rSegments[j].AirLine, pfSegment[j].AirLine));
                }
            }
            else
                errors.Append(FormatError(" Segment-Count", rSegments.Length.ToString(), pfSegment.Length.ToString()));


            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on PassengerInfoPage");
        }

        /// <summary>
        /// Extension method to validate air trip product with added product in cart from result page
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="airTripProduct">Air trip product on confirmation page</param>
        /// <param name="airResult">Added itinerary to cart on result page</param>
        public static void ValidateBookedTripProducts(this CheckoutPage page, AirTripProduct airTripProduct, AirResult airResult)
        {
            //Todo Implement Confirmation Page Validation
        }

        #endregion
    }
}
