namespace Rovia.UI.Automation.Tests.Validators
{
    using System;
    using System.Text;
    using Exceptions;
    using ScenarioObjects;
    using Pages;

    /// <summary>
    /// This class contains all the methods for hotel product validations
    /// </summary>
    public static class HotelValidator
    {
        #region Private Members

        private static string FormatError(string error, string addedValue, string tfValue)
        {
            return string.Format("| Invalid {0} ({1}, {2})", error, addedValue, tfValue);
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Extension method to validate hotel trip product with added product in cart from result page
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="tripProduct">Hotel trip product on trip folder page</param>
        /// <param name="hotelResult">Added itinerary to cart on result page</param>
        public static void ValidateTripProduct(this TripFolderPage page, HotelTripProduct tripProduct,HotelResult hotelResult)
        {
            var errors = new StringBuilder();
            if (!hotelResult.HotelName.Equals(tripProduct.ProductTitle, StringComparison.OrdinalIgnoreCase))
                errors.Append(FormatError("HotelName", hotelResult.HotelName, tripProduct.ProductTitle));
            if (!hotelResult.HotelAddress.Replace(",", "").Equals(tripProduct.Address.Replace(",", ""), StringComparison.OrdinalIgnoreCase))
                errors.Append(FormatError("HotelAddress", hotelResult.HotelAddress, tripProduct.Address));
            if (!hotelResult.Amount.Equals(tripProduct.Fares.TotalFare))
                errors.Append(FormatError("HotelPrice", hotelResult.Amount.ToString(), tripProduct.Fares.TotalFare.ToString()));
            if (!hotelResult.StayPeriod.Equals(tripProduct.StayPeriod))
                errors.Append(FormatError("StayPeriod", hotelResult.StayPeriod.ToString(), tripProduct.StayPeriod.ToString()));
            if (!hotelResult.Passengers.Equals(tripProduct.Passengers))
                errors.Append(FormatError("PassengersInfo", hotelResult.Passengers.ToString(), tripProduct.Passengers.ToString()));
            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on TripFolderPage");
        }

        /// <summary>
        /// Extension method to validate hotel trip product with added product in cart from result page
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="hotelTripProduct">Hotel trip product on passenger info page</param>
        /// <param name="hotelResult">Added itinerary to cart on result page</param>
        public static void ValidateTripProduct(this PassengerInfoPage page, HotelTripProduct hotelTripProduct, HotelResult hotelResult)
        {
            var errors = new StringBuilder();
            if (!hotelResult.HotelName.Equals(hotelTripProduct.ProductTitle, StringComparison.OrdinalIgnoreCase))
                errors.Append(FormatError("HotelName", hotelResult.HotelName, hotelTripProduct.ProductTitle));
            if (!hotelResult.HotelAddress.Replace(",", "").Equals(hotelTripProduct.Address.Replace(",", ""), StringComparison.OrdinalIgnoreCase))
                errors.Append(FormatError("HotelAddress", hotelResult.HotelAddress, hotelTripProduct.Address));
            if (!hotelResult.Amount.Equals(hotelTripProduct.Fares.TotalFare))
                errors.Append(FormatError("HotelPrice", hotelResult.Amount.ToString(), hotelTripProduct.Fares.TotalFare.ToString()));
            if (!hotelResult.StayPeriod.Equals(hotelTripProduct.StayPeriod))
                errors.Append(FormatError("StayPeriod", hotelResult.StayPeriod.ToString(), hotelTripProduct.StayPeriod.ToString()));
            if (!hotelResult.SelectedRoom.NoOfRooms.Equals(hotelTripProduct.Room.NoOfRooms))
                errors.Append(FormatError("NoOfRooms", hotelResult.SelectedRoom.NoOfRooms.ToString(), hotelTripProduct.Room.NoOfRooms.ToString()));
            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on PaxInfoPage");
        }

        /// <summary>
        /// Extension method to validate hotel trip product with added product in cart from result page
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="hotelTripProduct">Hotel trip product on checkout page</param>
        /// <param name="hotelResult">Added itinerary to cart on result page</param>
        public static void ValidateTripProduct(this CheckoutPage page, HotelTripProduct hotelTripProduct, HotelResult hotelResult)
        {
            var errors = new StringBuilder();
            if (!hotelResult.HotelName.Equals(hotelTripProduct.ProductTitle, StringComparison.OrdinalIgnoreCase))
                errors.Append(FormatError("HotelName", hotelResult.HotelName, hotelTripProduct.ProductTitle));
            if (!hotelResult.HotelAddress.Replace(",", "").Equals(hotelTripProduct.Address.Replace(",", ""), StringComparison.OrdinalIgnoreCase))
                errors.Append(FormatError("HotelAddress", hotelResult.HotelAddress, hotelTripProduct.Address));
            if (!hotelResult.Amount.Equals(hotelTripProduct.Fares.TotalFare))
                errors.Append(FormatError("HotelPrice", hotelResult.Amount.ToString(), hotelTripProduct.Fares.TotalFare.ToString()));
            if (!hotelResult.StayPeriod.Equals(hotelTripProduct.StayPeriod))
                errors.Append(FormatError("StayPeriod", hotelResult.StayPeriod.ToString(), hotelTripProduct.StayPeriod.ToString()));
            if (!hotelResult.SelectedRoom.NoOfRooms.Equals(hotelTripProduct.Room.NoOfRooms))
                errors.Append(FormatError("NoOfRooms", hotelResult.SelectedRoom.NoOfRooms.ToString(), hotelTripProduct.Room.NoOfRooms.ToString()));
            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on PaxInfoPage");
        }

        /// <summary>
        /// Extension method to validate hotel trip product with added product in cart from result page
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="hotelTripProduct">Hotel trip product on confirmation page</param>
        /// <param name="hotelResult">Added itinerary to cart on result page</param>
        public static void ValidateBookedTripProducts(this CheckoutPage page, HotelTripProduct hotelTripProduct, HotelResult hotelResult)
        {
            var errors = new StringBuilder();
            if (!hotelResult.HotelName.Equals(hotelTripProduct.ProductTitle, StringComparison.OrdinalIgnoreCase))
                errors.Append(FormatError("HotelName", hotelResult.HotelName, hotelTripProduct.ProductTitle));
            if (!hotelResult.HotelAddress.Replace(",", "").Equals(hotelTripProduct.Address.Replace(",", ""), StringComparison.OrdinalIgnoreCase))
                errors.Append(FormatError("HotelAddress", hotelResult.HotelAddress, hotelTripProduct.Address));
            if (!hotelResult.Amount.Equals(hotelTripProduct.Fares.TotalFare))
                errors.Append(FormatError("HotelPrice", hotelResult.Amount.ToString(), hotelTripProduct.Fares.TotalFare.ToString()));
            if (!hotelResult.StayPeriod.Equals(hotelTripProduct.StayPeriod))
                errors.Append(FormatError("StayPeriod", hotelResult.StayPeriod.ToString(), hotelTripProduct.StayPeriod.ToString()));
            if (!hotelResult.Passengers.Equals(hotelTripProduct.Passengers))
                errors.Append(FormatError("PassengersInfo", hotelResult.Passengers.ToString(), hotelTripProduct.Passengers.ToString()));
            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on ConfirmationPage");
        }

        #endregion
    }
}
