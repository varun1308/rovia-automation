namespace Rovia.UI.Automation.Tests.Pages
{
    using AppacitiveAutomationFramework;
    using ScenarioObjects;
    using Configuration;
    using Parsers;
    using Validators;

    /// <summary>
    /// This class holds all the fields and methods of trip folder page
    /// </summary>
    public class TripFolderPage : UIPage
    {
        public TripFolderParser TripProductParser { get; set; }

        /// <summary>
        /// Validate product on trip page to added itinerary in cart 
        /// </summary>
        /// <param name="selectedItineary">added itinerary to cart</param>
        internal void ValidateTripFolder(Results selectedItineary)
        {
            TripProductParser.ParseTripProducts().ForEach(x => this.ValidateTripProduct(x, selectedItineary));
        }

        /// <summary>
        /// Method to edit and save trip name
        /// </summary>
        internal void EditTripName()
        {
            WaitAndGetBySelector("tripSettings", ApplicationSettings.TimeOut.Fast).Click();

            WaitAndGetBySelector("editTripName", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("tripNameTextbox", ApplicationSettings.TimeOut.Slow).SendKeys("EditedTripName");

            //if wants to test cancel button/closing popup funcionality uncomment below & comment savetrip click 
            //WaitAndGetBySelector("cancelEditTripName",ApplicationSettings.TimeOut.Fast).Click();

            WaitAndGetBySelector("SaveTripName", ApplicationSettings.TimeOut.Fast).Click();
        }

        /// <summary>
        /// Method to Checkout Trip
        /// </summary>
        internal void CheckoutTrip()
        {
            WaitAndGetBySelector("checkoutButton", ApplicationSettings.TimeOut.Slow).Click();
        }

        /// <summary>
        /// Method for continue shopping
        /// </summary>
        internal void ContinueShopping()
        {
            WaitAndGetBySelector("continueShopping", ApplicationSettings.TimeOut.Slow).Click();
        }

        /// <summary>
        /// Method to reset the trip
        /// </summary>
        internal void TripStartOver()
        {
            WaitAndGetBySelector("tripSettings", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("startOver", ApplicationSettings.TimeOut.Fast).Click();
        }

        /// <summary>
        /// Method to save trip
        /// </summary>
        internal void SaveTrip()
        {
            WaitAndGetBySelector("tripSettings", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("saveTripButton", ApplicationSettings.TimeOut.Fast).Click();
        }
    }
}
