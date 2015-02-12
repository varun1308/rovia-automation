using Rovia.UI.Automation.Framework.Pages;
using System;
using System.Text;
using Rovia.UI.Automation.Exceptions;
using Rovia.UI.Automation.ScenarioObjects.Activity;

namespace Rovia.UI.Automation.Framework.Validators
{
    /// <summary>
    /// This class contains all the methods for activity product validations
    /// </summary>
    public static class ActivityValidator
    {
        #region Private Members

        private static string FormatError(string error, string addedValue, string tfValue)
        {
            return string.Format("| Invalid {0} ({1}, {2})", error, addedValue, tfValue);
        }

        #endregion

        #region Public Members

        /// <summary>
        /// Extension method to validate activity trip product with added product in cart from result page
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="activityTripProduct">Activity trip product on trip folder page</param>
        /// <param name="activityResult">Added itinerary to cart on result page</param>
        public static void ValidateTripProduct(this TripFolderPage page, ActivityTripProduct activityTripProduct, ActivityResult activityResult)
        {
            var errors = new StringBuilder();
            if (!activityResult.Amount.Equals(activityTripProduct.Fares.TotalFare))
                errors.Append(FormatError("ActivityFare", activityResult.Amount.ToString(), activityTripProduct.Fares.TotalFare.ToString()));
            //if (!activityResult.Category.Equals(activityTripProduct.Category,StringComparison.OrdinalIgnoreCase))
            //    errors.Append(FormatError("ActivityCategory", activityResult.Category, activityTripProduct.Category));
            if (!activityResult.ProductName.Equals(activityTripProduct.ActivityProductName))
                errors.Append(FormatError("ActivityProductName", activityResult.ProductName, activityTripProduct.ActivityProductName));
            if (!activityResult.Name.Equals(activityTripProduct.ProductTitle))
                errors.Append(FormatError("ActivityName", activityResult.Name, activityTripProduct.ProductTitle));
            if (!activityResult.Date.Equals(activityTripProduct.Date))
                errors.Append(FormatError("Activity Date", activityResult.Date.ToShortDateString(), activityTripProduct.Date.ToShortDateString()));
            if (!activityResult.Passengers.Equals(activityTripProduct.Passengers))
                errors.Append(FormatError("PassengersCount", activityResult.Passengers.ToString(), activityTripProduct.Passengers.ToString()));
            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on TripFolderPage");
        }

        /// <summary>
        /// Extension method to validate activity trip product with added product in cart from result page
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="activityTripProduct">Activity trip product on Passenger Info page</param>
        /// <param name="activityResult">Added itinerary to cart on result page</param>
        public static void ValidateTripProduct(this PassengerInfoPage page, ActivityTripProduct activityTripProduct, ActivityResult activityResult)
        {
            var errors = new StringBuilder();
            if (!activityResult.Name.Equals(activityTripProduct.ProductTitle, StringComparison.OrdinalIgnoreCase))
                errors.Append(FormatError("ActivityName", activityResult.Name, activityTripProduct.ProductTitle));
            //if (!activityResult.Category.Replace(",", "").Equals(activityTripProduct.Category.Replace(",", ""), StringComparison.OrdinalIgnoreCase))
            //    errors.Append(FormatError("ActivityCategory", activityResult.Category, activityTripProduct.Category));
            if (!activityResult.Amount.Equals(activityTripProduct.Fares.TotalFare))
                errors.Append(FormatError("ActivityPrice", activityResult.Amount.ToString(), activityTripProduct.Fares.TotalFare.ToString()));
            if (!activityResult.Date.Equals(activityTripProduct.Date))
                errors.Append(FormatError("ActvityDate", activityResult.Date.ToShortDateString(), activityTripProduct.Date.ToShortDateString()));
            if (!activityResult.ProductName.Equals(activityTripProduct.ActivityProductName, StringComparison.OrdinalIgnoreCase))
                errors.Append(FormatError("ActivityProductName", activityResult.ProductName, activityTripProduct.ActivityProductName));
            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on PaxInfoPage");
        }

        /// <summary>
        /// Extension method to validate activity trip product with added product in cart from result page
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="activityTripProduct">Activity trip product on checkout page</param>
        /// <param name="activityResult">Added itinerary to cart on result page</param>
        public static void ValidateTripProduct(this CheckoutPage page, ActivityTripProduct activityTripProduct, ActivityResult activityResult)
        {
            var errors = new StringBuilder();
            if (!activityResult.Name.Equals(activityTripProduct.ProductTitle, StringComparison.OrdinalIgnoreCase))
                errors.Append(FormatError("ActivityName", activityResult.Name, activityTripProduct.ProductTitle));
            //if (!activityResult.Category.Replace(",", "").Equals(activityTripProduct.Category.Replace(",", ""), StringComparison.OrdinalIgnoreCase))
            //    errors.Append(FormatError("ActivityCategory", activityResult.Category, activityTripProduct.Category));
            if (!activityResult.Amount.Equals(activityTripProduct.Fares.TotalFare))
                errors.Append(FormatError("ActivityPrice", activityResult.Amount.ToString(), activityTripProduct.Fares.TotalFare.ToString()));
            if (!activityResult.Date.Equals(activityTripProduct.Date))
                errors.Append(FormatError("ActvityDate", activityResult.Date.ToShortDateString(), activityTripProduct.Date.ToShortDateString()));
            if (!activityResult.ProductName.Equals(activityTripProduct.ActivityProductName, StringComparison.OrdinalIgnoreCase))
                errors.Append(FormatError("ActivityProductName", activityResult.ProductName, activityTripProduct.ActivityProductName));
            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on CheckoutPage");
        }

        /// <summary>
        /// Extension method to validate activity trip product with added product in cart from result page
        /// </summary>
        /// <param name="page">Page instance</param>
        /// <param name="activityTripProduct">Activity trip product on confirmation page</param>
        /// <param name="activityResult">Added itinerary to cart on result page</param>
        public static void ValidateBookedTripProducts(this CheckoutPage page, ActivityTripProduct activityTripProduct, ActivityResult activityResult)
        {
            var errors = new StringBuilder();
            if (!activityResult.Amount.Equals(activityTripProduct.Fares.TotalFare))
                errors.Append(FormatError("ActivityFare", activityResult.Amount.ToString(), activityTripProduct.Fares.TotalFare.ToString()));
            //if (!activityResult.Category.Equals(activityTripProduct.Category,StringComparison.OrdinalIgnoreCase))
            //    errors.Append(FormatError("ActivityCategory", activityResult.Category, activityTripProduct.Category));
            if (!activityResult.ProductName.Equals(activityTripProduct.ActivityProductName))
                errors.Append(FormatError("ActivityProductName", activityResult.ProductName, activityTripProduct.ActivityProductName));
            if (!activityResult.Name.Equals(activityTripProduct.ProductTitle))
                errors.Append(FormatError("ActivityName", activityResult.Name, activityTripProduct.ProductTitle));
            if (!activityResult.Date.Equals(activityTripProduct.Date))
                errors.Append(FormatError("Activity Date", activityResult.Date.ToShortDateString(), activityTripProduct.Date.ToShortDateString()));
            if (!activityResult.Passengers.Equals(activityTripProduct.Passengers))
                errors.Append(FormatError("PassengersCount", activityResult.Passengers.ToString(), activityTripProduct.Passengers.ToString()));
            if (!string.IsNullOrEmpty(errors.ToString()))
                throw new ValidationException(errors + "| on ConfirmationPage");
        }

        #endregion
    }
}
