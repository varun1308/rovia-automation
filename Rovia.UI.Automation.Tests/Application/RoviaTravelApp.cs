namespace Rovia.UI.Automation.Tests.Application
{
    using ScenarioObjects;
    using Pages;
    using Pages.SearchPanels;

    /// <summary>
    /// Travel site specific intialization
    /// </summary>
    public class RoviaTravelApp:RoviaApp
    {
        public override HomePage HomePage
        {
            get
            {
                var homePage = InitializePage<HomePage>("TravelHomeControls");
                switch (State.CurrentProduct)
                {
                    case TripProductType.Air: homePage.SearchPanel = InitializePage<TravelAirSearchPanel>("TravelAirSearchPanelControls");
                        break;
                    case TripProductType.Hotel: homePage.SearchPanel = InitializePage<TravelHotelSearchPanel>("TravelHotelSearchPanelControls");
                        break;
                    case TripProductType.Car: homePage.SearchPanel = InitializePage<TravelCarSearchPanel>("TravelCarSearchPanelControls");
                        break;
                    case TripProductType.Activity: homePage.SearchPanel = InitializePage<TravelActivitySearchPanel>("TravelActivitySearchPanelControls");
                        break;
                }
                return homePage;
            }
        }
    }
}
