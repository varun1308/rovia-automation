namespace Rovia.UI.Automation.Tests.Utility
{
    using System;
    using System.Collections.Generic;
    using Exceptions;
    using ScenarioObjects;

    /// <summary>
    /// This class provides conversion methods
    /// </summary>
    public static class UtilityFunctions
    {
        #region Private Members

        private static double GetGap(string day1, string day2)
        {
            var day = new[] { "MON", "TUE", "WED", "THU", "FRI", "SAT", "SUN" };
            var i = 0;
            while (true)
            {
                if (day[i].Equals(day1))
                    break;
                i++;
            }
            var k = i;
            while (true)
            {
                if (day[i % 7].Equals(day2))
                    break;
                i++;
            }
            return i - k;
        }

        private static TripProductType ToTripProductType(this string productType)
        {
            switch (productType.ToLower())
            {
                case "air":
                case "flight":
                    return TripProductType.Air;
                case "car":
                    return TripProductType.Car;
                case "hotel":
                    return TripProductType.Hotel;
                default:
                    throw new InvalidInputException(productType + " To UtilityFunctions.ToTripPRoductType");
            }
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Converts string cabin type to enumeration type
        /// </summary>
        /// <param name="cabinType"></param>
        /// <returns></returns>
        public static CabinType ToCabinType(this string cabinType)
        {
            switch (cabinType.Split()[0].ToLower())
            {
                case "economy":
                    return CabinType.Economy;
                case "premium_economy":
                case "premium economy":
                case "premium economy (z)":
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

        /// <summary>
        /// Converts string representation of time to TimeSpan object
        /// </summary>
        /// <param name="timeSpan">time in string</param>
        /// <returns></returns>
        public static TimeSpan ToTimeSpan(this string timeSpan)
        {
            try
            {
                if (string.IsNullOrEmpty(timeSpan))
                    return TimeSpan.Zero;
                var time = timeSpan.Replace("hr", ":").Replace("min", "").Replace(" ", "").Split(':');
                var hr = int.Parse(time[0].Trim());
                var min = time.Length > 1 ? int.Parse(time[1].Trim()) : 0;
                return new TimeSpan(hr, min, 0);
            }
            catch (Exception exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get dates with datetime format
        /// </summary>
        /// <returns></returns>
        public static DateTime[] GetDates(List<string> timeStrings, DateTime seed)
        {
            try
            {
                var flightDates = new DateTime[timeStrings.Count];
                flightDates[0] = DateTime.Parse(timeStrings[0] + " " + seed.ToShortDateString());
                for (var i = 1; i < flightDates.Length; i++)
                {
                    flightDates[i] =
                        DateTime.Parse(timeStrings[i] + " " +
                                       flightDates[i - 1].AddDays(GetGap(timeStrings[i - 1].Trim().Substring(0, 3).ToUpper(), timeStrings[i].Trim().Substring(0, 3).ToUpper())).ToShortDateString());
                }
                return flightDates;
            }
            catch (Exception exception)
            {

                throw;
            }
        }

        #endregion
    }
}
