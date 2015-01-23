using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.ScenarioObjects.Activity;
using Rovia.UI.Automation.Tests.Pages;

namespace Rovia.UI.Automation.Tests.Validators
{
    public static class Validator
    {
        public static void ValidateTripProduct(this TripFolderPage tripFolderPage,
                                               TripProduct tripProduct, Results results)
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

        public static void ValidateTripProduct(this PassengerInfoPage  passengerInfoPage,
                                               TripProduct tripProduct, Results results)
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

        public static void ValidateTripProduct(this CheckoutPage checkoutPage,
                                               TripProduct tripProduct, Results results)
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

        public static void ValidateBookedTripProducts(this CheckoutPage checkoutPage,
                                               TripProduct tripProduct, Results results)
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
