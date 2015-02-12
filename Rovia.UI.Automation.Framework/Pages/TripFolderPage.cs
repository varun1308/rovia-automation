using Rovia.UI.Automation.Framework.Configurations;
using AppacitiveAutomationFramework;
using Rovia.UI.Automation.Framework.Validators;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Framework.Pages
{
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
        public void ValidateTripFolder(Results selectedItineary)
        {
            TripProductParser.ParseTripProducts().ForEach(x => this.ValidateTripProduct(x, selectedItineary));
        }

        /// <summary>
        /// Method to edit and save trip name
        /// </summary>
        public void EditTripName()
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
        public void CheckoutTrip()
        {
            WaitAndGetBySelector("checkoutButton", ApplicationSettings.TimeOut.Slow).Click();
        }

        /// <summary>
        /// Method for continue shopping
        /// </summary>
        public void ContinueShopping()
        {
            WaitAndGetBySelector("continueShopping", ApplicationSettings.TimeOut.Slow).Click();
        }

        /// <summary>
        /// Method to reset the trip
        /// </summary>
        public void TripStartOver()
        {
            WaitAndGetBySelector("tripSettings", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("startOver", ApplicationSettings.TimeOut.Fast).Click();
        }

        /// <summary>
        /// Method to save trip
        /// </summary>
        public void SaveTrip()
        {
            WaitAndGetBySelector("tripSettings", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("saveTripButton", ApplicationSettings.TimeOut.Fast).Click();
        }
    }
}
