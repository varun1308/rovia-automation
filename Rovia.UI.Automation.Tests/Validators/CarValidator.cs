namespace Rovia.UI.Automation.Tests.Validators
{
    using System.Text;
    using Exceptions;
    using ScenarioObjects;
    using Pages;

    /// <summary>
    /// This class contains all the methods for car product validations
    /// </summary>
    public static class CarValidator
    {
        #region Private Members

        private static string FormatError(string error, string addedValue, string tfValue)
        {
            return string.Format("| Invalid {0} ({1}, {2})", error, addedValue, tfValue);
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Extension method to validate car trip product with added product in cart from result page
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="carTripProduct">Car trip product on trip folder page</param>
        /// <param name="carResult">Added itinerary to cart on result page</param>
        public static void ValidateTripProduct(this TripFolderPage page, CarTripProduct carTripProduct, CarResult carResult)
        {
            var errors = new StringBuilder();
            if (!carResult.TotalPrice.Equals(carTripProduct.Fares.TotalFare))
                errors.Append(FormatError("CarFare", carResult.TotalPrice.ToString(),
                                          carTripProduct.Fares.TotalFare.ToString()));
            if (!carResult.CarType.Equals(carTripProduct.CarType))
                errors.Append(FormatError("CarType", carResult.CarType, carTripProduct.CarType));
            if (!carResult.AirConditioning.Equals(carTripProduct.AirConditioning))
                errors.Append(FormatError("AirConditioning", carResult.AirConditioning, carTripProduct.AirConditioning));
            if (!carResult.Transmission.Equals(carTripProduct.Transmission))
                errors.Append(FormatError("Transmission", carResult.Transmission, carTripProduct.Transmission));
            if (!carResult.PickUpDateTime.Equals(carTripProduct.PickUpDateTime))
                errors.Append(FormatError("Pick Up DateTime", carResult.PickUpDateTime.ToLongDateString(),
                                          carTripProduct.PickUpDateTime.ToLongDateString()));
            if (!carResult.DropOffDateTime.Equals(carTripProduct.DropOffDateTime))
                errors.Append(FormatError("Drop Off DateTime", carResult.DropOffDateTime.ToLongDateString(),
                                          carTripProduct.DropOffDateTime.ToLongDateString()));
            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on TripFolderPage");
        }

        /// <summary>
        /// Extension method to validate car trip product with added product in cart from result page
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="carTripProduct">Car trip product on passenger info page</param>
        /// <param name="carResult">Added itinerary to cart on result page</param>
        public static void ValidateTripProduct(this PassengerInfoPage page, CarTripProduct carTripProduct, CarResult carResult)
        {
            var errors = new StringBuilder();
            if (!carResult.TotalPrice.Equals(carTripProduct.Fares.TotalFare))
                errors.Append(FormatError("CarFare", carResult.TotalPrice.ToString(), carTripProduct.Fares.TotalFare.ToString()));
            if (!carResult.CarType.Equals(carTripProduct.CarType))
                errors.Append(FormatError("CarType", carResult.CarType, carTripProduct.CarType));
            if (!carResult.RentalAgency.Equals(carTripProduct.RentalAgency))
                errors.Append(FormatError("RentalAgency", carResult.RentalAgency, carTripProduct.RentalAgency));
            if (!carResult.PickUpDateTime.Equals(carTripProduct.PickUpDateTime))
                errors.Append(FormatError("Pick Up DateTime", carResult.PickUpDateTime.ToLongDateString(), carTripProduct.PickUpDateTime.ToLongDateString()));
            if (!carResult.DropOffDateTime.Equals(carTripProduct.DropOffDateTime))
                errors.Append(FormatError("Drop Off DateTime", carResult.DropOffDateTime.ToLongDateString(), carTripProduct.DropOffDateTime.ToLongDateString()));
            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on PassengerInfoPage");
        }

        /// <summary>
        /// Extension method to validate car trip product with added product in cart from result page
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="carTripProduct">Car trip product on checkout page</param>
        /// <param name="carResult">Added itinerary to cart on result page</param>
        public static void ValidateTripProduct(this CheckoutPage page, CarTripProduct carTripProduct, CarResult carResult)
        {
            var errors = new StringBuilder();
            if (!carResult.TotalPrice.Equals(carTripProduct.Fares.TotalFare))
                errors.Append(FormatError("CarFare", carResult.TotalPrice.ToString(), carTripProduct.Fares.TotalFare.ToString()));
            if (!carResult.CarType.Equals(carTripProduct.CarType))
                errors.Append(FormatError("CarType", carResult.CarType, carTripProduct.CarType));
            if (!carResult.RentalAgency.Equals(carTripProduct.RentalAgency))
                errors.Append(FormatError("RentalAgency", carResult.RentalAgency, carTripProduct.RentalAgency));
            if (!carResult.PickUpDateTime.Equals(carTripProduct.PickUpDateTime))
                errors.Append(FormatError("Pick Up DateTime", carResult.PickUpDateTime.ToLongDateString(), carTripProduct.PickUpDateTime.ToLongDateString()));
            if (!carResult.DropOffDateTime.Equals(carTripProduct.DropOffDateTime))
                errors.Append(FormatError("Drop Off DateTime", carResult.DropOffDateTime.ToLongDateString(), carTripProduct.DropOffDateTime.ToLongDateString()));
            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on CheckOutPage");
        }

        /// <summary>
        /// Extension method to validate car trip product with added product in cart from result page
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="carTripProduct">Car trip product on confirmation page</param>
        /// <param name="carResult">Added itinerary to cart on result page</param>
        public static void ValidateBookedTripProducts(this CheckoutPage page, CarTripProduct carTripProduct, CarResult carResult)
        {
            var errors = new StringBuilder();
            if (!carResult.TotalPrice.Equals(carTripProduct.Fares.TotalFare))
                errors.Append(FormatError("CarFare", carResult.TotalPrice.ToString(), carTripProduct.Fares.TotalFare.ToString()));
            if (!carResult.CarType.Equals(carTripProduct.CarType))
                errors.Append(FormatError("CarType", carResult.CarType, carTripProduct.CarType));
            if (!carResult.AirConditioning.Equals(carTripProduct.AirConditioning))
                errors.Append(FormatError("AirConditioning", carResult.AirConditioning, carTripProduct.AirConditioning));
            if (!carResult.Transmission.Equals(carTripProduct.Transmission))
                errors.Append(FormatError("Transmission", carResult.Transmission, carTripProduct.Transmission));
            if (!carResult.PickUpDateTime.Equals(carTripProduct.PickUpDateTime))
                errors.Append(FormatError("Pick Up DateTime", carResult.PickUpDateTime.ToLongDateString(), carTripProduct.PickUpDateTime.ToLongDateString()));
            if (!carResult.DropOffDateTime.Equals(carTripProduct.DropOffDateTime))
                errors.Append(FormatError("Drop Off DateTime", carResult.DropOffDateTime.ToLongDateString(), carTripProduct.DropOffDateTime.ToLongDateString()));
            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on ConfirmationPage");
        }

        #endregion
    }
}
