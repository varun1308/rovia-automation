using Rovia.UI.Automation.ScenarioObjects;
using Rovia.UI.Automation.Tests.Pages;
using Rovia.UI.Automation.Tests.Pages.ResultPageComponents;
using Rovia.UI.Automation.Tests.Pages.ResultPageComponents.Activity;
using Rovia.UI.Automation.Tests.Pages.SearchPanels;

namespace Rovia.UI.Automation.Tests.Application
{
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
