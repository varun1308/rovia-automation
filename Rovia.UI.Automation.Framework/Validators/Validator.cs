using Rovia.UI.Automation.Framework.Pages;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.ScenarioObjects.Activity;

namespace Rovia.UI.Automation.Framework.Validators
{
    /// <summary>
    /// This class contains all the methods product validations on page redirections
    /// </summary>
    public static class Validator
    {
        /// <summary>
        /// Extension method to validate trip product with added product in cart from result page
        /// </summary>
        /// <param name="tripFolderPage">Page instance</param>
        /// <param name="tripProduct">Trip product on trip folder page</param>
        /// <param name="results">Added itinerary to cart on result page</param>
        public static void ValidateTripProduct(this TripFolderPage tripFolderPage,TripProduct tripProduct, Results results)
        {
            if (tripProduct is AirTripProduct)
                AirValidator.ValidateTripProduct(tripFolderPage, tripProduct as AirTripProduct, results as AirResult);
            else if (tripProduct is HotelTripProduct)
                HotelValidator.ValidateTripProduct(tripFolderPage, tripProduct as HotelTripProduct, results as HotelResult);
            else if (tripProduct is CarTripProduct)
                CarValidator.ValidateTripProduct(tripFolderPage, tripProduct as CarTripProduct, results as CarResult);
            else if (tripProduct is ActivityTripProduct)
                ActivityValidator.ValidateTripProduct(tripFolderPage, tripProduct as ActivityTripProduct, results as ActivityResult);
        }

        /// <summary>
        /// Extension method to validate trip product with added product in cart from result page
        /// </summary>
        /// <param name="passengerInfoPage">Page instance</param>
        /// <param name="tripProduct">Trip product on passenger info page</param>
        /// <param name="results">Added itinerary to cart on result page</param>
        public static void ValidateTripProduct(this PassengerInfoPage  passengerInfoPage,TripProduct tripProduct, Results results)
        {
            if (tripProduct is AirTripProduct)
                AirValidator.ValidateTripProduct(passengerInfoPage, tripProduct as AirTripProduct, results as AirResult);
            else if (tripProduct is HotelTripProduct)
                HotelValidator.ValidateTripProduct(passengerInfoPage, tripProduct as HotelTripProduct, results as HotelResult);
            else if (tripProduct is CarTripProduct)
                CarValidator.ValidateTripProduct(passengerInfoPage, tripProduct as CarTripProduct, results as CarResult);
            else if (tripProduct is ActivityTripProduct)
                ActivityValidator.ValidateTripProduct(passengerInfoPage, tripProduct as ActivityTripProduct, results as ActivityResult);
        }

        /// <summary>
        /// Extension method to validate trip product with added product in cart from result page
        /// </summary>
        /// <param name="checkoutPage">Page instance</param>
        /// <param name="tripProduct">Trip product on checkout page</param>
        /// <param name="results">Added itinerary to cart on result page</param>
        public static void ValidateTripProduct(this CheckoutPage checkoutPage,TripProduct tripProduct, Results results)
        {
            if (tripProduct is AirTripProduct)
                AirValidator.ValidateTripProduct(checkoutPage, tripProduct as AirTripProduct, results as AirResult);
            else if (tripProduct is HotelTripProduct)
                HotelValidator.ValidateTripProduct(checkoutPage, tripProduct as HotelTripProduct, results as HotelResult);
            else if (tripProduct is CarTripProduct)
                CarValidator.ValidateTripProduct(checkoutPage, tripProduct as CarTripProduct, results as CarResult);
            else if (tripProduct is ActivityTripProduct)
                ActivityValidator.ValidateTripProduct(checkoutPage, tripProduct as ActivityTripProduct, results as ActivityResult);
        }

        /// <summary>
        /// Extension method to validate trip product with added product in cart from result page
        /// </summary>
        /// <param name="checkoutPage">Page instance</param>
        /// <param name="tripProduct">Trip product on confirmation page</param>
        /// <param name="results">Added itinerary to cart on result page</param>
        public static void ValidateBookedTripProducts(this CheckoutPage checkoutPage,TripProduct tripProduct, Results results)
        {
            if (tripProduct is AirTripProduct)
                AirValidator.ValidateBookedTripProducts(checkoutPage, tripProduct as AirTripProduct, results as AirResult);
            else if (tripProduct is HotelTripProduct)
                HotelValidator.ValidateBookedTripProducts(checkoutPage, tripProduct as HotelTripProduct, results as HotelResult);
            else if (tripProduct is CarTripProduct)
                CarValidator.ValidateBookedTripProducts(checkoutPage, tripProduct as CarTripProduct, results as CarResult);
            else if (tripProduct is ActivityTripProduct)
                ActivityValidator.ValidateBookedTripProducts(checkoutPage, tripProduct as ActivityTripProduct, results as ActivityResult);
        }
    }
}
