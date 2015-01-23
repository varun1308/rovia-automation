using System;

namespace Rovia.UI.Automation.ScenarioObjects
{
    public class AirportPair
    {
        public AirportPair(AirportPair airportPair)
        {
            ArrivalAirport = airportPair.ArrivalAirport;
            DepartureAirport = airportPair.DepartureAirport;
            ArrivalDateTime = airportPair.ArrivalDateTime;
            DepartureDateTime = airportPair.DepartureDateTime;
        }

        public AirportPair()
        {
        }

        public override bool Equals(object airportPair)
        {
            var other = airportPair as AirportPair;

            return other != null && (string.Equals(DepartureAirport, other.DepartureAirport, StringComparison.OrdinalIgnoreCase) && string.Equals(ArrivalAirport, other.ArrivalAirport, StringComparison.OrdinalIgnoreCase) && DepartureDateTime.Equals(other.DepartureDateTime) && ArrivalDateTime.Equals(other.ArrivalDateTime));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (DepartureAirport != null ? DepartureAirport.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ArrivalAirport != null ? ArrivalAirport.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ DepartureDateTime.GetHashCode();
                hashCode = (hashCode*397) ^ ArrivalDateTime.GetHashCode();
                return hashCode;
            }
        }

        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public DateTime DepartureDateTime { get; set; }
        public DateTime ArrivalDateTime { get; set; }

        public override string ToString()
        {
            return string.Format("From {0} at {1}:{2} to {3} at {4}:{5}", DepartureAirport,
                                 DepartureDateTime.ToShortDateString(), DepartureDateTime.ToShortTimeString(),
                                 ArrivalAirport, ArrivalDateTime.ToShortDateString(),
                                 ArrivalDateTime.ToShortTimeString());
        }
    }


}
