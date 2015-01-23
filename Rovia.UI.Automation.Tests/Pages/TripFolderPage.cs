using AppacitiveAutomationFramework;
using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Configuration;
using Rovia.UI.Automation.Tests.Pages.Parsers;
using Rovia.UI.Automation.Tests.Validators;

namespace Rovia.UI.Automation.Tests.Pages
{
    public class TripFolderPage : UIPage
    {
        public TripFolderParser TripProductParser { get; set; }
        
        internal void EditTripName()
        {
            WaitAndGetBySelector("tripSettings", ApplicationSettings.TimeOut.Fast).Click();

            WaitAndGetBySelector("editTripName", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("tripNameTextbox", ApplicationSettings.TimeOut.Slow).SendKeys("EditedTripName");

            //if wants to test cancel button/closing popup funcionality uncomment below & comment savetrip click 
            //WaitAndGetBySelector("cancelEditTripName",ApplicationSettings.TimeOut.Fast).Click();

            WaitAndGetBySelector("SaveTripName", ApplicationSettings.TimeOut.Fast).Click();
        }

        internal void ValidateTripFolder(Results selectedItineary)
        {
           TripProductParser.ParseTripProducts().ForEach(x=>this.ValidateTripProduct(x,selectedItineary));
        }

        internal void CheckoutTrip()
        {
            WaitAndGetBySelector("checkoutButton", ApplicationSettings.TimeOut.Slow).Click();
        }

        internal void ContinueShopping()
        {
            WaitAndGetBySelector("continueShopping", ApplicationSettings.TimeOut.Slow).Click();
        }

        internal void TripStartOver()
        {
            WaitAndGetBySelector("tripSettings", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("startOver", ApplicationSettings.TimeOut.Fast).Click();
        }

        internal void SaveTrip()
        {
            WaitAndGetBySelector("tripSettings", ApplicationSettings.TimeOut.Fast).Click();
            WaitAndGetBySelector("saveTripButton", ApplicationSettings.TimeOut.Fast).Click();
        }
    }
}
