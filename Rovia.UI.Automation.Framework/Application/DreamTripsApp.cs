﻿using Rovia.UI.Automation.Framework.Pages;
using Rovia.UI.Automation.ScenarioObjects;

namespace Rovia.UI.Automation.Framework.Application
{
    /// <summary>
    /// Dreamtrips.com site specific intialization
    /// </summary>
    public  class DreamTripsApp :RoviaApp
    {
        public override HomePage HomePage
        {
            get
            {
                var homePage = InitializePage<HomePage>("DTHomeControls");
                switch (State.CurrentProduct)
                {
                    case TripProductType.Air: homePage.SearchPanel = InitializePage<DTAirSearchPanel>("DTAirSearchPanelControls");
                        break;
                    case TripProductType.Hotel: homePage.SearchPanel = InitializePage<DTHotelSearchPanel>("DTHotelSearchPanelControls");
                        break;
                    case TripProductType.Car: homePage.SearchPanel = InitializePage<DTCarSearchPanel>("DTCarSearchPanelControls");
                        break;
                    case TripProductType.Activity: homePage.SearchPanel = InitializePage<DTActivitySearchPanel>("DTActivitySearchPanelControls");
                        break;
                }
                return homePage;
            }
        }
    }
}
