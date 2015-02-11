namespace Rovia.UI.Automation.Tests.Validators
{
    using ScenarioObjects;
    using ScenarioObjects.Activity;
    using Pages;

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
                tripFolderPage.ValidateTripProduct(tripProduct as AirTripProduct, results as AirResult);
            else if (tripProduct is HotelTripProduct)
                tripFolderPage.ValidateTripProduct(tripProduct as HotelTripProduct, results as HotelResult);
            else if (tripProduct is CarTripProduct)
                tripFolderPage.ValidateTripProduct(tripProduct as CarTripProduct, results as CarResult);
            else if (tripProduct is ActivityTripProduct)
                tripFolderPage.ValidateTripProduct(tripProduct as ActivityTripProduct, results as ActivityResult);
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
                passengerInfoPage.ValidateTripProduct(tripProduct as AirTripProduct, results as AirResult);
            else if (tripProduct is HotelTripProduct)
                passengerInfoPage.ValidateTripProduct(tripProduct as HotelTripProduct, results as HotelResult);
            else if (tripProduct is CarTripProduct)
                passengerInfoPage.ValidateTripProduct(tripProduct as CarTripProduct, results as CarResult);
            else if (tripProduct is ActivityTripProduct)
                passengerInfoPage.ValidateTripProduct(tripProduct as ActivityTripProduct, results as ActivityResult);
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
                checkoutPage.ValidateTripProduct(tripProduct as AirTripProduct, results as AirResult);
            else if (tripProduct is HotelTripProduct)
                checkoutPage.ValidateTripProduct(tripProduct as HotelTripProduct, results as HotelResult);
            else if (tripProduct is CarTripProduct)
                checkoutPage.ValidateTripProduct(tripProduct as CarTripProduct, results as CarResult);
            else if (tripProduct is ActivityTripProduct)
                checkoutPage.ValidateTripProduct(tripProduct as ActivityTripProduct, results as ActivityResult);
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
                checkoutPage.ValidateBookedTripProducts(tripProduct as AirTripProduct, results as AirResult);
            else if (tripProduct is HotelTripProduct)
                checkoutPage.ValidateBookedTripProducts(tripProduct as HotelTripProduct, results as HotelResult);
            else if (tripProduct is CarTripProduct)
                checkoutPage.ValidateBookedTripProducts(tripProduct as CarTripProduct, results as CarResult);
            else if (tripProduct is ActivityTripProduct)
                checkoutPage.ValidateBookedTripProducts(tripProduct as ActivityTripProduct, results as ActivityResult);
        }
    }
}
